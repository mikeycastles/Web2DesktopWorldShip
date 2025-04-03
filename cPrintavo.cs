using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebAppShipping
{
    public static class PrintavoApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _authToken = string.Empty;
        private static DateTime _tokenExpiration = DateTime.MinValue;
        private static AppSettings _settings;

        private static readonly Dictionary<string, string> StateNameToAbbreviation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Alabama", "AL" },
            { "Alaska", "AK" },
            { "Arizona", "AZ" },
            { "Arkansas", "AR" },
            { "California", "CA" },
            { "Colorado", "CO" },
            { "Connecticut", "CT" },
            { "Delaware", "DE" },
            { "Florida", "FL" },
            { "Georgia", "GA" },
            { "Hawaii", "HI" },
            { "Idaho", "ID" },
            { "Illinois", "IL" },
            { "Indiana", "IN" },
            { "Iowa", "IA" },
            { "Kansas", "KS" },
            { "Kentucky", "KY" },
            { "Louisiana", "LA" },
            { "Maine", "ME" },
            { "Maryland", "MD" },
            { "Massachusetts", "MA" },
            { "Michigan", "MI" },
            { "Minnesota", "MN" },
            { "Mississippi", "MS" },
            { "Missouri", "MO" },
            { "Montana", "MT" },
            { "Nebraska", "NE" },
            { "Nevada", "NV" },
            { "New Hampshire", "NH" },
            { "New Jersey", "NJ" },
            { "New Mexico", "NM" },
            { "New York", "NY" },
            { "North Carolina", "NC" },
            { "North Dakota", "ND" },
            { "Ohio", "OH" },
            { "Oklahoma", "OK" },
            { "Oregon", "OR" },
            { "Pennsylvania", "PA" },
            { "Rhode Island", "RI" },
            { "South Carolina", "SC" },
            { "South Dakota", "SD" },
            { "Tennessee", "TN" },
            { "Texas", "TX" },
            { "Utah", "UT" },
            { "Vermont", "VT" },
            { "Virginia", "VA" },
            { "Washington", "WA" },
            { "West Virginia", "WV" },
            { "Wisconsin", "WI" },
            { "Wyoming", "WY" }
        };

        static PrintavoApiClient()
        {
            try
            {
                _settings = new AppSettings();
                if (string.IsNullOrEmpty(_settings.Email) || string.IsNullOrEmpty(_settings.Password))
                {
                    throw new Exception("Settings could not be loaded. Email or Password is empty.");
                }
                SetCredentials(_settings.Email, _settings.Password);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to initialize PrintavoApiClient with settings.", ex);
            }
        }

        public static void ReloadSettings()
        {
            _settings = new AppSettings();
            SetCredentials(_settings.Email, _settings.Password);
        }

        private static void SetCredentials(string email, string password)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                _settings.Email = email;
                _settings.Password = password;
            }
        }

        public static async Task<bool> LoginAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.Email) || string.IsNullOrEmpty(_settings.Password))
                    throw new Exception("Email and password are not set.");

                var loginData = new { email = _settings.Email, password = _settings.Password };
                var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_settings.BaseUrl + "sessions", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                    _authToken = result.token;
                    _tokenExpiration = DateTime.UtcNow.AddHours(1);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                    Console.WriteLine("Login successful. Token stored.");
                    return true;
                }

                Console.WriteLine("Login failed.");
                return false;
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return false;
            }
        }

        private static async Task EnsureAuthenticatedAsync()
        {
            if (string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiration)
            {
                Console.WriteLine("Authentication required. Logging in...");
                if (!await LoginAsync())
                {
                    throw new Exception("Failed to authenticate.");
                }
            }
        }

        public static async Task<List<Order>> GetAllOrdersForDateAsync(string date)
        {
            try
            {
                await EnsureAuthenticatedAsync();
                var orders = new List<Order>();
                int page = 1, totalPages = 1;

                do
                {
                    string url = $"{_settings.BaseUrl}orders?page={page}&per_page=25&inProductionAfter={date}T00:00:00Z&inProductionBefore={date}T23:59:59Z";
                    var response = await FetchOrdersAsync(url);
                    if (response != null)
                    {
                        orders.AddRange(response.data);
                        totalPages = response.meta.total_pages;
                    }
                    page++;
                } while (page <= totalPages);

                return orders;
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return new List<Order>();
            }
        }

        public static async Task<string> ImportOrdersAsync(string date)
        {
            try
            {
                List<Order> orders = await GetAllOrdersForDateAsync(date);
                Console.WriteLine($"Total Orders Retrieved: {orders.Count}");

                if (AccessHelper.TableExists("Orders"))
                    AccessHelper.ExecuteNonQuery("DELETE FROM Orders");

                foreach (var order in orders)
                {
                    Console.WriteLine($"Order ID: {order.id}, Customer: {order.customer.first_name}, Total: {order.order_total}");
                    Customer customer = await GetCustomerByIdAsync(order.customer_id);

                    if (string.IsNullOrWhiteSpace(customer.company))
                        customer.company = $"{customer.first_name} {customer.last_name}";

                    dynamic invoice = new ExpandoObject();
                    var orderDict = (IDictionary<string, object>)invoice;

                    orderDict["OrderID"] = order.id;
                    orderDict["Company"] = customer.company;
                    orderDict["Address1"] = customer.billing_address_attributes.address1;
                    orderDict["Address2"] = customer.billing_address_attributes.address2;
                    orderDict["City"] = customer.billing_address_attributes.city;
                    orderDict["State"] = ConvertStateNameToAbbreviation(customer.billing_address_attributes.state);
                    orderDict["Zip"] = customer.billing_address_attributes.zip;
                    orderDict["Country"] = customer.billing_address_attributes.country;

                    var values = orderDict.Select(kv => (kv.Key, kv.Value)).ToArray();
                    AccessHelper.InsertOrUpdate("Orders", "OrderID", orderDict["OrderID"], values);
                }

                DataTable dt2 = AccessHelper.ExecuteQuery("SELECT * FROM Orders");
                return dt2?.Rows.Count.ToString() + " Orders Imported Successfully!" ?? "Error retrieving orders.";
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return "Error importing orders.";
            }
        }

        public static async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            await EnsureAuthenticatedAsync();
            string url = $"{_settings.BaseUrl}customers/{customerId}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                json = json.Replace("\"id\": 1194978", "\"id\": 1194978,");
                return JsonConvert.DeserializeObject<Customer>(json);
            }
            else
            {
                throw new Exception($"Error retrieving customer info. Status: {response.StatusCode}");
            }
        }

        private static async Task<ApiResponse> FetchOrdersAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse>(json);
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return null;
            }
        }

        public static async Task<bool> UpdateOrderNotesAsync(int orderId, string newNotes)
        {
            await EnsureAuthenticatedAsync();
            string url = $"{_settings.BaseUrl}orders/{orderId}";

            var getResponse = await _httpClient.GetAsync(url);
            if (!getResponse.IsSuccessStatusCode)
                throw new Exception($"Error retrieving order with id {orderId}. Status: {getResponse.StatusCode}");

            string json = await getResponse.Content.ReadAsStringAsync();
            JObject orderData = JObject.Parse(json);
            orderData["notes"] = newNotes;
            string updatedJson = orderData.ToString();

            var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");
            var putResponse = await _httpClient.PutAsync(url, content);
            if (!putResponse.IsSuccessStatusCode)
                throw new Exception($"Error updating order notes. Status: {putResponse.StatusCode}");

            Console.WriteLine("Order notes updated successfully.");
            return true;
        }

        public static async Task<string> ProcessShippedOrdersAsync()
        {
            int totalExported = 0;
            try
            {
                if (!_settings.Tracking)
                {
                    var dt = AccessHelper.ExecuteQuery("SELECT * FROM [shipped] WHERE [trackingnumber] IS NOT NULL AND [trackingnumber] <> '' AND [sent] = 0");
                    if (dt == null || dt.Rows.Count == 0)
                        return "No shipped orders with tracking numbers found to process.";

                    var groups = dt.AsEnumerable().GroupBy(r => Convert.ToInt32(r["orderid"]));
                    foreach (var group in groups)
                    {
                        try
                        {
                            int orderId = group.Key;
                            var trackingNumbers = string.Join(", ", group.Select(r => r["trackingnumber"].ToString()).Distinct());
                            if (await UpdateOrderNotesAsync(orderId, trackingNumbers))
                            {
                                AccessHelper.ExecuteNonQuery("UPDATE [shipped] SET [sent] = ? WHERE [orderid] = ?", ("sent", true), ("orderid", orderId));
                                totalExported += trackingNumbers.Split(',').Length;
                            }
                        }
                        catch (Exception ex)
                        {
                            EventLogHelper.WriteErrorLog(ex);
                        }
                    }

                }

                return $"{totalExported} tracking number(s) exported.";
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return "Error processing shipped orders.";
            }
        }

        private static string ConvertStateNameToAbbreviation(string stateName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stateName)) return stateName;
                stateName = stateName.Trim();
                return StateNameToAbbreviation.TryGetValue(stateName, out var abbr) ? abbr : stateName;
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return stateName;
            }
        }
    }
}

#region Model Classes

public class Meta
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total_count { get; set; }
        public int total_pages { get; set; }
    }

    public class Stats
    {
        public bool paid { get; set; }
    }

    public class AddressAttributes
    {
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public DateTime created_at { get; set; }
        public int id { get; set; }
        public string state { get; set; }
        public DateTime updated_at { get; set; }
        public string zip { get; set; }
    }

    public class SubCustomer
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int user_id { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public int customer_id { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int user_id { get; set; }
        public string fax { get; set; }
        public int orders_count { get; set; }
        public bool tax_exempt { get; set; }
        public string tax_resale_num { get; set; }
        public string company { get; set; }
        public string customer_email { get; set; }
        public string phone { get; set; }
        public string extra_notes { get; set; }
        public int default_payment_term_id { get; set; }
        public int customer_id { get; set; }
        public AddressAttributes shipping_address_attributes { get; set; }
        public AddressAttributes billing_address_attributes { get; set; }
        public List<SubCustomer> sub_customers_attributes { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string url { get; set; }
        public string public_page_url { get; set; }
    }

    public class User
    {
        public string name { get; set; }
    }

    public class OrderStatus
    {
        public string name { get; set; }
    }

    public class Order
    {
        public int id { get; set; }
        public string sales_tax { get; set; }
        public int customer_id { get; set; }
        public int user_id { get; set; }
        public Customer customer { get; set; }
        public User user { get; set; }
        public OrderStatus orderstatus { get; set; }
        public Stats stats { get; set; }
        public string order_total { get; set; }
        public string invoice_date { get; set; }
        public string url { get; set; }
    }

    public class ApiResponse
    {
        public Meta meta { get; set; }
        public List<Order> data { get; set; }
    }

    #endregion
