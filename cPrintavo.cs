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

namespace WebAppShipping
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json; // Make sure you have the Newtonsoft.Json package
    using Newtonsoft.Json.Linq;

    // Assume EventLogHelper is available in your project

    public static class PrintavoApiClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static string _authToken = string.Empty; // Stores the API token
        private static DateTime _tokenExpiration = DateTime.MinValue; // Token expiration time
        private static string _email = string.Empty; // Store login email globally
        private static string _password = string.Empty; // Store login password globally

        private static readonly string BaseUrl = "https://private-anon-66b928c4b9-printavo.apiary-mock.com/api/v1/";

        /// <summary>
        /// Sets the user credentials (email & password) for login.
        /// </summary>
        public static void SetCredentials(string email, string password)
        {
            try
            {
                _email = email;
                _password = password;
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
            }
        }

        /// <summary>
        /// Logs in using stored credentials and retrieves the API token.
        /// </summary>
        public static async Task<bool> LoginAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password))
                {
                    throw new Exception("Email and password are not set. Use SetCredentials() before logging in.");
                }

                var loginData = new { email = _email, password = _password };
                var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl + "sessions", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                    _authToken = result.token;
                    _tokenExpiration = DateTime.UtcNow.AddHours(1); // Adjust if needed
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

        /// <summary>
        /// Ensures the user is authenticated before making API requests.
        /// </summary>
        private static async Task EnsureAuthenticatedAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_authToken) || DateTime.UtcNow >= _tokenExpiration)
                {
                    Console.WriteLine("Authentication required. Logging in...");
                    bool loginSuccess = await LoginAsync();
                    if (!loginSuccess)
                    {
                        throw new Exception("Failed to authenticate.");
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all orders for a specific date from Printavo API.
        /// </summary>
        public static async Task<List<Order>> GetAllOrdersForDateAsync(string date)
        {
            try
            {
                await EnsureAuthenticatedAsync(); // Ensure login before fetching orders

                List<Order> allOrders = new List<Order>();
                int currentPage = 1;
                int totalPages = 1;

                do
                {
                    string url = $"{BaseUrl}orders?page={currentPage}&per_page=25&inProductionAfter={date}T00:00:00Z&inProductionBefore={date}T23:59:59Z";
                    Console.WriteLine($"Fetching page {currentPage}...");

                    ApiResponse response = await FetchOrdersAsync(url);

                    if (response != null)
                    {
                        allOrders.AddRange(response.data);
                        totalPages = response.meta.total_pages;
                    }

                    currentPage++;

                } while (currentPage <= totalPages);

                return allOrders;
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return new List<Order>(); // Return an empty list on error
            }
        }

        /// <summary>
        /// Retrieves customer information by customer ID asynchronously.
        /// </summary>
        public static async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                await EnsureAuthenticatedAsync();

                // Build the request URL for customer info.
                string url = $"{BaseUrl}customers/{customerId}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    // Temp fix: address API sandbox bug (missing comma in a child object property)
                    json = json.Replace("\"id\": 1194978", "\"id\": 1194978,");
                    Customer customer = JsonConvert.DeserializeObject<Customer>(json);
                    return customer;
                }
                else
                {
                    throw new Exception($"Error retrieving customer info. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Makes the HTTP request for orders and deserializes the response.
        /// </summary>
        private static async Task<ApiResponse> FetchOrdersAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return null;
                }

                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse>(json);
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return null;
            }
        }
        /// <summary>
        ///Instead of deserializing to a dynamic object, we parse the JSON string into a JObject.This preserves all existing fields—even those not defined in our class model.
        /// TO get around this, we capture the object into 
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="newNotes"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateOrderNotesAsync(int orderId, string newNotes)
        {
            try
            {
                // Ensure we're authenticated
                await EnsureAuthenticatedAsync();

                // Construct the URL for the specific order.
                string url = $"{BaseUrl}orders/{orderId}";

                // Retrieve the existing order.
                HttpResponseMessage getResponse = await _httpClient.GetAsync(url);
                if (!getResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Error retrieving order with id {orderId}. Status: {getResponse.StatusCode}");
                }
                string json = await getResponse.Content.ReadAsStringAsync();

                // Parse the JSON into a JObject to capture all fields.
                JObject orderData = JObject.Parse(json);

                // Update the 'notes' property.
                orderData["notes"] = newNotes;

                // Convert the modified JObject back into a JSON string.
                string updatedJson = orderData.ToString();

                // Create the content for the PUT request.
                var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");

                // Send a PUT request to update the order.
                HttpResponseMessage putResponse = await _httpClient.PutAsync(url, content);
                if (!putResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Error updating order notes. Status: {putResponse.StatusCode}");
                }

                Console.WriteLine("Order notes updated successfully.");
                return true;
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return false;
            }
        }

        public static async Task<string> ProcessShippedOrdersAsync()
        {
            int totalExportedTrackingNumbers = 0;
            try
            {
                // Query the shipped table.
                DataTable dt = AccessHelper.ExecuteQuery(
                    "SELECT * FROM [shipped] WHERE [trackingnumber] IS NOT NULL AND [trackingnumber] <> '' AND [sent] = 0");

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Group rows by order id.
                    var ordersGroup = dt.AsEnumerable().GroupBy(r => Convert.ToInt32(r["orderid"]));
                    foreach (var orderGroup in ordersGroup)
                    {
                        try
                        {
                            int orderId = orderGroup.Key;
                            // Get distinct tracking numbers for this order.
                            var trackingNumbersList = orderGroup
                                .Select(r => r["trackingnumber"].ToString())
                                .Distinct()
                                .ToList();
                            string trackingNumbers = string.Join(", ", trackingNumbersList);

                            // Update the order's notes with the joined tracking numbers.
                            bool updateSuccess = await UpdateOrderNotesAsync(orderId, trackingNumbers);
                            if (updateSuccess)
                            {
                                // Mark all shipped records for this order as sent.
                                AccessHelper.ExecuteNonQuery(
                                    "UPDATE [shipped] SET [sent] = ? WHERE [orderid] = ?",
                                    ("sent", true),
                                    ("orderid", orderId));

                                // Increase count by the number of distinct tracking numbers.
                                totalExportedTrackingNumbers += trackingNumbersList.Count;
                            }
                        }
                        catch (Exception innerEx)
                        {
                            // Log errors for individual order groups and continue.
                            EventLogHelper.WriteErrorLog(innerEx);
                        }
                    }
                    return $"{totalExportedTrackingNumbers} tracking number(s) exported.";
                }
                else
                {
                    return "No shipped orders with tracking numbers found to process.";
                }
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return "Error processing shipped orders.";
            }
        }




    }

    public static class cPrintavo
    {
        /// <summary>
        /// Retrieves orders for the specified date, imports them into the local database, and returns a status message.
        /// </summary>
        public static async Task<string> GetOrders(string strDate)
        {
            try
            {
                // Retrieve settings from the local database.
                DataTable dt = AccessHelper.ExecuteQuery("SELECT * FROM Settings");
                if (dt != null && dt.Rows.Count > 0)
                {
                    // Assumes settings table columns: [id], [username], [password] (adjust indexes as needed)
                    PrintavoApiClient.SetCredentials(dt.Rows[0][1].ToString(), dt.Rows[0][2].ToString());

                    List<Order> orders = await PrintavoApiClient.GetAllOrdersForDateAsync(strDate);
                    Console.WriteLine($"Total Orders Retrieved: {orders.Count}");

                    // Delete existing orders, if any.
                    if (AccessHelper.TableExists("Orders"))
                        AccessHelper.ExecuteNonQuery("DELETE FROM Orders");

                    foreach (var order in orders)
                    {
                        Console.WriteLine($"Order ID: {order.id}, Customer: {order.customer.first_name}, Total: {order.order_total}");
                        Customer customer = await PrintavoApiClient.GetCustomerByIdAsync(order.customer_id);

                        dynamic invoice = new ExpandoObject();
                        var orderDict = (IDictionary<string, object>)invoice;
                        if (customer.company.Length == 0)
                            customer.company = customer.first_name + " " + customer.last_name;

                        // Build a dictionary with order and customer info.
                        orderDict["OrderID"] = order.id;
                        orderDict["Company"] = customer.company;
                        orderDict["Address1"] = customer.billing_address_attributes.address1;
                        orderDict["Address2"] = customer.billing_address_attributes.address2;
                        orderDict["City"] = customer.billing_address_attributes.city;
                        orderDict["State"] = ConvertStateNameToAbbreviation(customer.billing_address_attributes.state);
                        orderDict["Zip"] = customer.billing_address_attributes.zip;
                        orderDict["Country"] = customer.billing_address_attributes.country;

                        // Convert the dynamic object to an array of (string, object) pairs.
                        var values = orderDict.Select(kv => (kv.Key, kv.Value)).ToArray();

                        // Insert or update the order in the database.
                        AccessHelper.InsertOrUpdate("Orders", "OrderID", orderDict["OrderID"], values);
                    }
                }
                else
                {
                    Console.WriteLine("No settings found in the database.");
                }

                DataTable dt2 = AccessHelper.ExecuteQuery("SELECT * FROM Orders");
                if (dt2 == null)
                {
                    return "Error retrieving orders.";
                }
                return dt2.Rows.Count.ToString() + " Orders Imported Successfully!";
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                throw;
                // Alternatively, you might return an error message:
                // return "Error importing orders: " + ex.Message;
            }
        }

        // Dictionary mapping full state names to abbreviations.
        private static readonly Dictionary<string, string> StateNameToAbbreviation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        {"Alabama", "AL"},
        {"Alaska", "AK"},
        {"Arizona", "AZ"},
        {"Arkansas", "AR"},
        {"California", "CA"},
        {"Colorado", "CO"},
        {"Connecticut", "CT"},
        {"Delaware", "DE"},
        {"Florida", "FL"},
        {"Georgia", "GA"},
        {"Hawaii", "HI"},
        {"Idaho", "ID"},
        {"Illinois", "IL"},
        {"Indiana", "IN"},
        {"Iowa", "IA"},
        {"Kansas", "KS"},
        {"Kentucky", "KY"},
        {"Louisiana", "LA"},
        {"Maine", "ME"},
        {"Maryland", "MD"},
        {"Massachusetts", "MA"},
        {"Michigan", "MI"},
        {"Minnesota", "MN"},
        {"Mississippi", "MS"},
        {"Missouri", "MO"},
        {"Montana", "MT"},
        {"Nebraska", "NE"},
        {"Nevada", "NV"},
        {"New Hampshire", "NH"},
        {"New Jersey", "NJ"},
        {"New Mexico", "NM"},
        {"New York", "NY"},
        {"North Carolina", "NC"},
        {"North Dakota", "ND"},
        {"Ohio", "OH"},
        {"Oklahoma", "OK"},
        {"Oregon", "OR"},
        {"Pennsylvania", "PA"},
        {"Rhode Island", "RI"},
        {"South Carolina", "SC"},
        {"South Dakota", "SD"},
        {"Tennessee", "TN"},
        {"Texas", "TX"},
        {"Utah", "UT"},
        {"Vermont", "VT"},
        {"Virginia", "VA"},
        {"Washington", "WA"},
        {"West Virginia", "WV"},
        {"Wisconsin", "WI"},
        {"Wyoming", "WY"}
    };

        /// <summary>
        /// Converts a full U.S. state name to its two-letter abbreviation.
        /// If the state name is not recognized, returns the original string.
        /// </summary>
        public static string ConvertStateNameToAbbreviation(string stateName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(stateName))
                {
                    return stateName;
                }

                stateName = stateName.Trim();

                if (StateNameToAbbreviation.TryGetValue(stateName, out string abbreviation))
                {
                    return abbreviation;
                }
                else
                {
                    return stateName;
                }
            }
            catch (Exception ex)
            {
                EventLogHelper.WriteErrorLog(ex);
                return stateName;
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
}
