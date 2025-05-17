using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Http
{
    public class HttpGetFunction : IFsFunction
    {
        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol}: too few parameters. Expected at least 1, got {pars.Length}.");

            var url = pars[0]?.ToString();
            if (string.IsNullOrWhiteSpace(url))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{Symbol}: invalid URL.");

            var queryParams = new Dictionary<string, string>();
            if (pars.Length >= 2 && pars[1] is FsList qpList)
            {
                for (int i = 0; i < qpList.Length; i += 2)
                {
                    var key = qpList[i]?.ToString();
                    var value = (i + 1 < qpList.Length) ? qpList[i + 1]?.ToString() : "";
                    if (!string.IsNullOrEmpty(key))
                        queryParams[key] = value;
                }
            }
            if (queryParams.Count > 0)
            {
                var queryString = string.Join("&", queryParams.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
                url += url.Contains("?") ? "&" + queryString : "?" + queryString;
            }

            var headers = new Dictionary<string, string>();
            if (pars.Length >= 3 && pars[2] is FsList headerList)
            {
                for (int i = 0; i < headerList.Length; i += 2)
                {
                    var key = headerList[i]?.ToString();
                    var value = (i + 1 < headerList.Length) ? headerList[i + 1]?.ToString() : "";
                    if (!string.IsNullOrEmpty(key))
                        headers[key] = value;
                }
            }

            try
            {
                using var client = new HttpClient();
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }

                var response = client.GetAsync(url).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                    return new FsError(FsError.ERROR_DEFAULT,
                        $"{Symbol}: HTTP GET failed with status code: {response.StatusCode}: {responseString}");

                return responseString;
            }
            catch (Exception ex)
            {
                return new FsError(FsError.ERROR_DEFAULT,
                    $"{Symbol}: Exception occurred - {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "url",
                1 => "query_params (optional)",
                2 => "headers (optional)",
                _ => ""
            };
        }

        public CallType CallType => CallType.Prefix;
        public string Symbol => "HttpGet";
    }
}