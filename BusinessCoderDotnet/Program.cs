using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Cryptography;

namespace BusinessCoderDotnet
{
  /// <summary>
  /// Business Coder appends a Melissa Address Key (MAK) and other identifiers to a business
  /// record based on its name and address, letting you link and de-duplicate business data
  /// across systems.
  ///
  /// <para>High-level flow of this sample:</para>
  /// <list type="number">
  ///   <item><description>ARGS    - ParseArguments reads any --flag values off the command line.</description></item>
  ///   <item><description>INPUT   - CallAPI fills in whatever wasn't supplied via interactive prompts.</description></item>
  ///   <item><description>REQUEST - CallAPI builds the REST query string (license + input fields).</description></item>
  ///   <item><description>CALL    - GetContents issues the GET request and pretty-prints the JSON response.</description></item>
  /// </list>
  ///
  /// <para>This sample is a thin HTTP client: it builds a query string, sends a GET
  /// request to the Business Coder Cloud API, and prints the JSON response.</para>
  ///
  /// <para>Reference:</para>
  /// <list type="bullet">
  ///   <item><description>Documentation: https://docs.melissa.com/cloud-api/business-coder/business-coder-index.html</description></item>
  ///   <item><description>Release notes: https://releasenotes.melissa.com/cloud-api/business-coder/</description></item>
  ///   <item><description>Result codes: https://docs.melissa.com/melissa/result-codes/result-codes-index.html</description></item>
  /// </list>
  /// </summary>
  static class Program
  {
    /// <summary>
    /// Entry point. Reads the optional command-line arguments, then hands control to
    /// CallAPI, which performs the actual request/response cycle.
    /// </summary>
    /// <param name="args">The raw command-line arguments.</param>
    static void Main(string[] args)
    {
      string baseServiceUrl = @"https://businesscoder.melissadata.net/";
      string serviceEndpoint = @"WEB/BusinessCoder/doBusinessCoderUS"; //please see https://www.melissa.com/developer/business-coder for more endpoints
      string license = "";
      string company = "";
      string addressline1 = "";
      string city = "";
      string state = "";
      string postal = "";
      string country = "";

      // Populate any values passed on the command line, then run the lookup.
      ParseArguments(ref license, ref company, ref addressline1, ref city, ref state, ref postal, ref country, args);
      CallAPI(baseServiceUrl, serviceEndpoint, license, company, addressline1, city, state, postal, country);
    }

    /// <summary>
    /// Reads the supported command-line options and writes each recognized value into
    /// its matching by-ref parameter. Any parameter left unset here falls back to an
    /// interactive prompt later in <see cref="CallAPI"/>.
    ///
    /// <para>Recognized flags (each followed by its value, e.g. "--company Melissa Data"):
    /// --license/-l, --company, --addressline1, --city, --state, --postal, --country.</para>
    /// </summary>
    /// <param name="license">Receives the Melissa license string, if supplied.</param>
    /// <param name="company">Receives the business/company name to test, if supplied.</param>
    /// <param name="addressline1">Receives the street address to test, if supplied.</param>
    /// <param name="city">Receives the city to test, if supplied.</param>
    /// <param name="state">Receives the state to test, if supplied.</param>
    /// <param name="postal">Receives the postal code to test, if supplied.</param>
    /// <param name="country">Receives the country to test, if supplied.</param>
    /// <param name="args">The raw command-line arguments to parse.</param>
    static void ParseArguments(ref string license, ref string company, ref string addressline1, ref string city, ref string state, ref string postal, ref string country, string[] args)
    {
      for (int i = 0; i < args.Length; i++)
      {
        if (args[i].Equals("--license") || args[i].Equals("-l"))
        {
          if (args[i + 1] != null)
          {
            license = args[i + 1];
          }
        }
        if (args[i].Equals("--company"))
        {
          if (args[i + 1] != null)
          {
            company = args[i + 1];
          }
        }
        if (args[i].Equals("--addressline1"))
        {
          if (args[i + 1] != null)
          {
            addressline1 = args[i + 1];
          }
        }
        if (args[i].Equals("--city"))
        {
          if (args[i + 1] != null)
          {
            city = args[i + 1];
          }
        }
        if (args[i].Equals("--state"))
        {
          if (args[i + 1] != null)
          {
            state = args[i + 1];
          }
        }
        if (args[i].Equals("--postal"))
        {
          if (args[i + 1] != null)
          {
            postal = args[i + 1];
          }
        }
        if (args[i].Equals("--country"))
        {
          if (args[i + 1] != null)
          {
            country = args[i + 1];
          }
        }
      }
    }

    /// <summary>
    /// Issues the GET request against the Business Coder endpoint and pretty-prints
    /// the API call and the JSON response to the console.
    /// </summary>
    /// <param name="baseServiceUrl">The Business Coder Cloud API base URL.</param>
    /// <param name="requestQuery">The endpoint path plus query string built by <see cref="CallAPI"/>.</param>
    public static async Task GetContents(string baseServiceUrl, string requestQuery)
    {
      HttpClient client = new HttpClient();
      client.BaseAddress = new Uri(baseServiceUrl);
      HttpResponseMessage response = await client.GetAsync(requestQuery);

      string text = await response.Content.ReadAsStringAsync();

      // Re-serialize with indentation so the raw response is easier to read.
      var obj = JsonConvert.DeserializeObject(text);
      var prettyResponse = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);

      // Print output
      Console.WriteLine("\n================================== OUTPUT ==================================\n");

      Console.WriteLine("API Call: ");
      string APICall = Path.Combine(baseServiceUrl, requestQuery);
      for (int i = 0; i < APICall.Length; i += 70)
      {
        if (i + 70 < APICall.Length)
        {
          Console.WriteLine(APICall.Substring(i, 70));
        }
        else
        {
          Console.WriteLine(APICall.Substring(i, APICall.Length - i));
        }
      }

      Console.WriteLine("\nAPI Response:");
      Console.WriteLine(prettyResponse);
    }

    /// <summary>
    /// Drives the interactive/CLI loop: gathers the required lookup fields, builds and
    /// submits the REST query, prints the result, and optionally repeats for another record.
    ///
    /// <para>In interactive mode (no lookup args) it loops, asking for a new record each pass
    /// until the user answers "N". In one-shot mode (lookup args supplied) it runs a single
    /// pass and exits.</para>
    /// </summary>
    /// <param name="baseServiceUrl">The Business Coder Cloud API base URL.</param>
    /// <param name="serviceEndPoint">The specific Business Coder endpoint path to call.</param>
    /// <param name="license">The Melissa license string sent with every request.</param>
    /// <param name="company">A company name to test in one-shot mode; if empty, the program prompts interactively.</param>
    /// <param name="addressline1">A street address to test in one-shot mode.</param>
    /// <param name="city">A city to test in one-shot mode.</param>
    /// <param name="state">A state to test in one-shot mode.</param>
    /// <param name="postal">A postal code to test in one-shot mode.</param>
    /// <param name="country">A country to test in one-shot mode.</param>
    static void CallAPI(string baseServiceUrl, string serviceEndPoint, string license, string company, string addressline1, string city, string state, string postal, string country)
    {
      Console.WriteLine("\n================ WELCOME TO MELISSA BUSINESS CODER CLOUD API ===============\n");

      bool shouldContinueRunning = true;
      while (shouldContinueRunning)
      {
        string inputCompany = "";
        string inputAddressLine1 = "";
        string inputCity = "";
        string inputState = "";
        string inputPostal = "";
        string inputCountry = "";

        // No values were supplied via command line, so prompt for every field.
        if (string.IsNullOrEmpty(company) && string.IsNullOrEmpty(addressline1) && string.IsNullOrEmpty(city) && string.IsNullOrEmpty(state)
          && string.IsNullOrEmpty(postal) && string.IsNullOrEmpty(country))
        {
          Console.WriteLine("\nFill in each value to see results");

          Console.Write("Company: ");
          inputCompany = Console.ReadLine();

          Console.Write("AddressLine1: ");
          inputAddressLine1 = Console.ReadLine();

          Console.Write("City: ");
          inputCity = Console.ReadLine();

          Console.Write("State: ");
          inputState = Console.ReadLine();

          Console.Write("Postal: ");
          inputPostal = Console.ReadLine();

          Console.Write("Country: ");
          inputCountry = Console.ReadLine();
        }
        else
        {
          // At least one field was supplied via command line; use those values as-is.
          inputCompany = company;
          inputAddressLine1 = addressline1;
          inputCity = city;
          inputState = state;
          inputPostal = postal;
          inputCountry = country;
        }

        // Prompt individually for any still-missing required field.
        while (string.IsNullOrEmpty(inputCompany) || string.IsNullOrEmpty(inputAddressLine1) || string.IsNullOrEmpty(inputCity) || string.IsNullOrEmpty(inputState) || string.IsNullOrEmpty(inputPostal)
          || string.IsNullOrEmpty(inputCountry))
        {
          Console.WriteLine("\nFill in missing required parameter");

          if (string.IsNullOrEmpty(inputCompany))
          {
            Console.Write("Company: ");
            inputCompany = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputAddressLine1))
          {
            Console.Write("AddressLine1: ");
            inputAddressLine1 = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputCity))
          {
            Console.Write("City: ");
            inputCity = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputState))
          {
            Console.Write("State: ");
            inputState = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputPostal))
          {
            Console.Write("Postal: ");
            inputPostal = Console.ReadLine();
          }

          if (string.IsNullOrEmpty(inputCountry))
          {
            Console.Write("Country: ");
            inputCountry = Console.ReadLine();
          }
        }

        // Map input fields to the API's expected query parameter names.
        Dictionary<string, string> inputs = new Dictionary<string, string>()
        {
            { "comp", inputCompany},
            { "a1", inputAddressLine1},
            { "city", inputCity},
            { "state", inputState},
            { "postal", inputPostal},
            { "ctry", inputCountry},
        };

        Console.WriteLine("\n=================================== INPUTS =================================\n");
        Console.WriteLine($"\t   Base Service Url: {baseServiceUrl}");
        Console.WriteLine($"\t  Service End Point: {serviceEndPoint}");
        Console.WriteLine($"\t            Company: {inputCompany}");
        Console.WriteLine($"\t       AddressLine1: {inputAddressLine1}");
        Console.WriteLine($"\t               City: {inputCity}");
        Console.WriteLine($"\t              State: {inputState}");
        Console.WriteLine($"\t        Postal Code: {inputPostal}");
        Console.WriteLine($"\t            Country: {inputCountry}");

        // Create Service Call
        // Set the License String in the Request
        string RESTRequest = "";

        RESTRequest += @"&id=" + Uri.EscapeDataString(license);

        // Set the Input Parameters
        foreach (KeyValuePair<string, string> kvp in inputs)
          RESTRequest += @"&" + kvp.Key + "=" + Uri.EscapeDataString(kvp.Value);

        // Build the final REST String Query
        RESTRequest = serviceEndPoint + @"?" + RESTRequest;

        // Submit to the Web Service.
        bool success = false;
        int retryCounter = 0;

        do
        {
          try //retry just in case of network failure
          {
            GetContents(baseServiceUrl, $"{RESTRequest}").Wait();
            Console.WriteLine();
            success = true;
          }
          catch (Exception ex)
          {
            retryCounter++;
            Console.WriteLine(ex.ToString());
            return;
          }
        } while ((success != true) && (retryCounter < 5));

        // If any lookup field came from the command line, treat this as a one-shot
        // run rather than looping for additional records.
        bool isValid = false;
        if (!string.IsNullOrEmpty(company + addressline1 + city + state + postal + country))
        {
          isValid = true;
          shouldContinueRunning = false;
        }

        // Otherwise ask whether to test another record. Keep prompting until we get a
        // valid Y/N. "N" ends the program; "Y" falls through to another pass.
        while (!isValid)
        {
          Console.WriteLine("\nTest another record? (Y/N)");
          string testAnotherResponse = Console.ReadLine();

          if (!string.IsNullOrEmpty(testAnotherResponse))
          {
            testAnotherResponse = testAnotherResponse.ToLower();
            if (testAnotherResponse == "y")
            {
              isValid = true;
            }
            else if (testAnotherResponse == "n")
            {
              isValid = true;
              shouldContinueRunning = false;
            }
            else
            {
              Console.Write("Invalid Response, please respond 'Y' or 'N'");
            }
          }
        }
      }

      Console.WriteLine("\n==================== THANK YOU FOR USING MELISSA CLOUD API =================\n");
    }
  }
}
