using Mojio.Client.Linq;
using Mojio.Events;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Mojio.Client
{
    public class MapEntity
    {
        static Dictionary<Type, string> Map = new Dictionary<Type, string> ();

        public void Add (Type type, string controller)
        {
            Map.Add (type, controller);
        }

        public string this [Type type] {
            get {
                if (Map.ContainsKey (type))
                    return Map [type];

                foreach (var pair in Map) {
                    if (type.IsSubclassOf(pair.Key))
                    {
                        // Lets add it now... might be faster?
                        Add (type, pair.Value);
                        return pair.Value;
                    }
                }

                return null;
            }
            set {
                Add (type, value);
            }
        }
    }

    public partial class MojioClient : IMojioClient
    {
        public const string Sandbox = "https://sandbox.api.moj.io/v1";
        public const string Live = "https://api.moj.io/v1";
        public const string OAuthAuthorize = "https://api.moj.io/oauth2/authorize";
        public const string OAuthToken = "https://api.moj.io/oauth2/token";

        public int PageSize { get; set; }

        public int SessionTime { get; set; }

        RestClient RestClient;
        public Token Token { get; protected set; }
        protected static MapEntity Map = new MapEntity ();

        static MojioClient ()
        {
            Map.Add (typeof(App), "apps");
            Map.Add (typeof(User), "users");
            Map.Add (typeof(Mojio), "mojios");
            Map.Add (typeof(Vehicle), "vehicles");
            Map.Add (typeof(Event), "events");
            Map.Add(typeof(Access), "access");

            //Map.Add(typeof(GPSEvent), "events");
            //Map.Add(typeof(IgnitionEvent), "events");
            //Map.Add(typeof(ITripEvent), "events");
            //Map.Add(typeof(TripStatusEvent), "events");
            //Map.Add(typeof(HardEvent), "events");

            Map.Add (typeof(Trip), "trips");
            Map.Add (typeof(Product), "products");

            Map.Add (typeof(Invoice), "orders");

            Map.Add (typeof(Subscription), "subscriptions");

            Map.Add (typeof(Observer), "observe");
            Map.Add(typeof(Log), "logs");
        }

        [Obsolete("This constructor has been deprecated and may cause deadlocks.  Use BeginAsync to perform client intialization instead.")]
        public MojioClient(Guid tokenId, string Url = Live)
            : this (Url)
        {
            Begin(tokenId);
        }

        /// <summary>
        /// Begin a new session using the App ID and secretKey.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <param name="Url">API endpoint URL</param>
        [Obsolete("This constructor has been deprecated and may cause deadlocks.  Use BeginAsync to perform client intialization instead.")]
        public MojioClient (Guid appId, Guid secretKey, string Url = Live)
            : this (Url)
        {
            Begin (appId, secretKey);
        }

        /// <summary>
        /// Attempt to begin a connection using the tokenID.
        /// Begin a new session using the appId and secret key if token has expired.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <param name="tokenId">Session Token</param>
        /// <param name="Url">API endpoint URL</param>
        [Obsolete("This constructor has been deprecated and may cause deadlocks.  Use BeginAsync to perform client intialization instead.")]
        public MojioClient (Guid appId, Guid secretKey, Guid? tokenId, string Url = Live)
            : this (Url)
        {
            Begin (appId, secretKey, tokenId);
        }

        /// <summary>
        /// Authenticate a new session token with a specific user attached.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <param name="userOrEmail">User's name or email</param>
        /// <param name="passsword">User's password.</param>
        /// <param name="Url">API endpoint URL</param>
        [Obsolete("This constructor has been deprecated and may cause deadlocks.  Use BeginAsync to perform client intialization instead.")]
        public MojioClient (Guid appId, Guid secretKey, string userOrEmail, string password, string Url = Live)
            : this (Url)
        {
            Begin (appId, secretKey, userOrEmail, password);
        }

        /// <summary>
        /// Initiate a new MojioClient.  No session token has been created yet.
        /// Client must call BeginAsync, or supply set a valid Token.
        /// </summary>
        /// <param name="Url">API endpoint URL</param>
        public MojioClient (string Url = Live)
        {
            PageSize = 10;
            SessionTime = 24 * 60;

            RestClient = new RestClient (Url);
            RestClient.ClearHandlers ();
            RestClient.AddHandler ("application/json", new RSJsonSerializer ());
            RestClient.AddHandler ("*", new RSJsonSerializer ());
        }

        /// <summary>
        /// Build request URL string from controller/id/action/key
        /// </summary>
        /// <param name="controller">Controller name.</param>
        /// <param name="id">Object ID</param>
        /// <param name="action">Action Name</param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Request (params object[] args)
        {
            var str = "";
            foreach(var arg in args)
            {
                if (arg != null)
                    str += String.Format("{0}/", arg);
            }

            return str;
        }

        /// <summary>
        /// Continue a session with a given tokenID.
        /// </summary>
        /// <param name="tokenId">A valid session token</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Begin(Guid tokenId)
        {
            var request = GetRequest(Request("login", tokenId), Method.GET);
            var response = AvoidAsyncDeadlock(() => RequestAsync<Token>(request)).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Token = response.Data;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt to begin a connection using the tokenID.
        /// Begin a new session using the appId and secret key if token has expired.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <param name="tokenId">Session Token</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Begin(Guid appId, Guid secretKey, Guid? tokenId)
        {
            return AvoidAsyncDeadlock(() => BeginAsync(appId, secretKey, tokenId)).Result;
        }

        /// <summary>
        /// Attempt to begin a connection using the tokenID.
        /// Begin a new session using the appId and secret key if token has expired.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <param name="tokenId">Session Token</param>
        /// <returns></returns>
        public async Task<bool> BeginAsync(Guid appId, Guid secretKey, Guid? tokenId)
        {
            try {
                if (tokenId != null && tokenId != Guid.Empty) {
                    var request = GetRequest (Request ("login", tokenId.Value), Method.GET);
                    var response = await RequestAsync<Token>(request);

                    if (response.StatusCode == HttpStatusCode.OK && response.Data.AppId == appId)
                    {
                        Token = response.Data;
                        return true;
                    }
                }

                return await BeginAsync (appId, secretKey);
            } catch (Exception ex) {
                throw new Exception ("Exception" + ex.Message + "\n  Stack:\n" + ex.StackTrace.ToString () + "\n");
                //return false;
            }
        }

        /// <summary>
        /// Begin a new session using the appId and secret key.
        /// This session will not include an attached user.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Begin(Guid appId, Guid secretKey)
        {
            return AvoidAsyncDeadlock(() => BeginAsync(appId, secretKey)).Result;
        }

        /// <summary>
        /// Begin a new session using the appId and secret key.
        /// This session will not include an attached user.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <returns></returns>
        public async Task<bool> BeginAsync (Guid appId, Guid secretKey)
        {
            var request = GetRequest (Request ("login", appId), Method.POST);
            request.AddBody ("");
            request.AddParameter ("secretKey", secretKey);
            var response = await RequestAsync<Token> (request);
            if (response.StatusCode == HttpStatusCode.OK) {
                Token = response.Data;
                return true;
            } else if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException ("Invalid AppID/Secret Key");

            return false;
        }

        /// <summary>
        /// Begin a new API session using the AppID and Secret Key, aswell as attach a mojio user.
        /// </summary>
        /// <param name="appId">Application ID</param>
        /// <param name="secretKey">Secret Key</param>
        /// <param name="userOrEmail">User's name or email</param>
        /// <param name="password">User's password</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Begin (Guid appId, Guid secretKey, string userOrEmail, string password)
        {
            var request = GetRequest (Request ("login", appId), Method.POST);
            request.AddBody ("");
            request.AddParameter ("secretKey", secretKey);
            request.AddParameter ("userOrEmail", userOrEmail);
            request.AddParameter ("password", password);

            var response = AvoidAsyncDeadlock(() => RequestAsync<Token>(request)).Result;
            if (response.StatusCode == HttpStatusCode.OK) {
                Token = response.Data;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Authenticate a user and update the current session token.
        /// </summary>
        /// <param name="userOrEmail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool SetUser (string userOrEmail, string password)
        {
            HttpStatusCode ignore;
            return SetUser (userOrEmail, password, out ignore);
        }

        /// <summary>
        /// Authenticate a user and update the current session token.
        /// </summary>
        /// <param name="userOrEmail"></param>
        /// <param name="password"></param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool SetUser (string userOrEmail, string password, out HttpStatusCode code)
        {
            string ignore;
            return SetUser (userOrEmail, password, out code, out ignore);
        }

        /// <summary>
        /// Authenticate a user and update the current session token.
        /// </summary>
        /// <param name="userOrEmail"></param>
        /// <param name="password"></param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool SetUser (string userOrEmail, string password, out HttpStatusCode code, out string message)
        {
            var response = AvoidAsyncDeadlock(() => SetUserAsync(userOrEmail, password)).Result;
            code = response.StatusCode;
            message = response.Content;

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        public Task<MojioResponse<Token>> SetUserAsync (string userOrEmail, string password)
        {
            if (Token == null)
                throw new Exception ("Valid session must be initialized first."); // Can only "Login" if already authenticated app.

            var request = GetRequest (Request ("login", userOrEmail, "user"), Method.POST);
            request.AddBody ("");
            request.AddParameter ("userOrEmail", userOrEmail);
            request.AddParameter ("password", password);
            request.AddParameter ("minutes", SessionTime);

            var task = RequestAsync<Token> (request);

            return task.ContinueWith<MojioResponse<Token>> (r => {
                var response = r.Result;
                if (response != null && response.StatusCode == HttpStatusCode.OK) {
                    Token = response.Data;
                    ResetCurrentUser ();
                }

                return response;
            });
        }

        /// <summary>
        /// Unset authenticated user.
        /// </summary>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ClearUser ()
        {
            var response = AvoidAsyncDeadlock(() => ClearUserAsync()).Result;

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        public Task<MojioResponse<Token>> ClearUserAsync ()
        {
            if (Token == null)
                throw new Exception ("Valid session must be initialized first.");

            var request = GetRequest (Request ("login", Token.Id, "user"), Method.DELETE);

            var task = RequestAsync<Token> (request);
            return task.ContinueWith<MojioResponse<Token>> (r => {
                var response = r.Result;
                if (response.StatusCode == HttpStatusCode.OK) {
                    Token = response.Data;
                    ResetCurrentUser ();
                }

                return response;
            });
        }

        /// <summary>
        /// Extend token expiry date.
        /// </summary>
        /// <param name="minutes">Exipry time in minutes</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ExtendSession (int minutes)
        {
            HttpStatusCode ignore;
            return ExtendSession (minutes, out ignore);
        }

        /// <summary>
        /// Extend token expiry date.
        /// </summary>
        /// <param name="minutes">Exipry time in minutes</param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ExtendSession (int minutes, out HttpStatusCode code)
        {
            string ignore;
            return ExtendSession (minutes, out code, out ignore);
        }

        /// <summary>
        /// Extend token expiry date.
        /// </summary>
        /// <param name="minutes">Exipry time in minutes</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ExtendSession (int minutes, out HttpStatusCode code, out string message)
        {
            var response = AvoidAsyncDeadlock(() => ExtendSessionAsync(minutes)).Result;

            code = response.StatusCode;
            message = response.Content;

            return response.StatusCode == HttpStatusCode.OK;
        }

        public Task<MojioResponse<Token>> ExtendSessionAsync (int minutes)
        {
            if (Token == null)
                throw new Exception ("No session to extend."); // Can only "Extend" if already authenticated app.

            var request = GetRequest (Request ("login", Token.Id, "Session"), Method.POST);
            request.AddBody ("");
            request.AddParameter ("minutes", minutes);

            var task = RequestAsync<Token> (request);

            return task.ContinueWith<MojioResponse<Token>> (r => {
                var response = r.Result;
                if (response.StatusCode == HttpStatusCode.OK)
                    Token = response.Data;

                return response;
            });
        }

        public async Task<bool> ChangeEnvironmentAsync(bool sandboxed)
        {
            if (Token.Sandboxed != sandboxed) {
                var request = GetRequest(Request("login", Token.Id, "Sandboxed"), Method.PUT);
                request.AddBody ("");
                request.AddParameter("sandboxed", sandboxed);
                try
                {
                    var result = await RequestAsync<Token>(request);

                    var token = result.Data;

                    if(token != null) {
                        Token = token;
                        return true;
                    }else {
                        System.Diagnostics.Debug.WriteLine("Failed {0}: {1}", result.Content, result.ErrorMessage);
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }else{
                return true;
            }
        }

        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ChangeEnvironment(bool sandboxed)
        {
            var task = AvoidAsyncDeadlock(() => ChangeEnvironmentAsync(sandboxed));
            return task.Result;
        }

        /// <summary>
        /// Generate basic request. Adds the session token to header if exists.
        /// </summary>
        /// <param name="resource">Resource URL</param>
        /// <param name="method">Request method</param>
        /// <returns></returns>
        public CustomRestRequest GetRequest (string resource, Method method)
        {
            var request = new CustomRestRequest (resource, method);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RSJsonSerializer ();

            if (Token != null)
                request.AddHeader (Headers.MojioAPITokenHeader, Token.Id.ToString ());
            return request;
        }

        public async Task<MojioResponse> RequestAsync (RestRequest request)
        {
            return await RestClient.ExecuteAsync (request).ContinueWith(t => {
                var response = t.Result;
                if (response.StatusCode == 0)
                {
                    return new MojioResponse
                    {
                        ErrorMessage = response.ErrorMessage,
                        Content = response.Content,
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    return new MojioResponse
                    {
                        Content = response.Content,
                        StatusCode = response.StatusCode
                    };
                }
            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<MojioResponse<T>> RequestAsync<T> (RestRequest request)
            where T : new()
        {
            return await RestClient.ExecuteAsync<T>(request).ContinueWith(t =>
            {
                var response = t.Result;

                MojioResponse<T> r;
                if (response.StatusCode == 0)
                {
                    r = new MojioResponse<T>
                    {
                        ErrorMessage = response.ErrorMessage,
                        Content = response.Content,
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
                else
                {
                    r = new MojioResponse<T>
                    {
                        Data = response.Data,
                        Content = response.Content,
                        StatusCode = response.StatusCode
                    };

                    if (response.Data == null)
                    {
                        try
                        {
                            var error = Deserialize<String>(response.Content);
                            r.ErrorMessage = error;
                        }
                        catch (Exception)
                        {
                            // Exception thrown.  I don't think we need to do anything with it though.
                            r.ErrorMessage = response.Content;
                        }
                    }
                }
                return r;
            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        /// <summary>
        /// Create a new entity through the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Entity to create</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Create<T> (T entity)
            where T : BaseEntity, new()
        {
            HttpStatusCode ignore;
            return Create<T> (entity, out ignore);
        }

        /// <summary>
        /// Create a new entity through the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Entity to create</param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Create<T> (T entity, out HttpStatusCode code)
            where T : BaseEntity, new()
        {
            string ignore;
            return Create<T> (entity, out code, out ignore);
        }

        /// <summary>
        /// Create a new entity through the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Entity to create</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Create<T> (T entity, out HttpStatusCode code, out string message)
            where T : BaseEntity, new()
        {
            var response = AvoidAsyncDeadlock(() => CreateAsync(entity)).Result;
            code = response.StatusCode;
            message = response.Content;

            return response.Data;
        }

        public Task<MojioResponse<T>> CreateAsync<T> (T entity)
            where T : BaseEntity, new()
        {
            if (typeof(T) == typeof(User)) {
                // Cannot create user with this generic method
                throw new ArgumentException ("Cannot create user with generic method Create. Use RegisterUser instead.");
            }

            string action = Map [typeof(T)];

            var request = GetRequest (Request (action), Method.POST);

            request.AddBody (entity);

            return RequestAsync<T> (request);
        }

        public Task<MojioResponse<Mojio>> ClaimAsync (Mojio entity, int? pin)
        {
            return ClaimAsync (entity.Imei, pin);
        }

        public Task<MojioResponse<Mojio>> ClaimAsync (String imei, int? pin)
        {
            string controller = Map [typeof(Mojio)];

            var request = GetRequest (Request (controller, imei, "user"), Method.PUT);
            request.AddBody ("");
            request.AddParameter ("pin", pin);

            return RequestAsync<Mojio> (request);
        }

        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Unclaim(Guid id)
        {
            var response = AvoidAsyncDeadlock(() => UnclaimAsync(id)).Result;

            return response.Data;
        }

        public Task<MojioResponse<bool>> UnclaimAsync(Guid id)
        {
            string controller = Map[typeof(Mojio)];

            var request = GetRequest(Request(controller, id, "user"), Method.DELETE);

            return RequestAsync<bool>(request);
        }

        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool UpdatePin(Guid id, int pin)
        {
            var response = AvoidAsyncDeadlock(() => UpdatePinAsync(id, pin)).Result;

            return response.Data;
        }

        public Task<MojioResponse<bool>> UpdatePinAsync(Guid id, int pin)
        {
            string controller = Map[typeof(Mojio)];

            var request = GetRequest(Request(controller, id, "pin"), Method.PUT);
            request.AddBody ("");
            request.AddParameter("pin", pin);

            return RequestAsync<bool>(request);
        }

        /// <summary>
        /// Delete an entity through the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Entity to delete</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Delete<T> (T entity)
            where T : BaseEntity
        {
            HttpStatusCode ignore;
            return Delete<T> (entity, out ignore);
        }

        /// <summary>
        /// Delete an entity through the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Entity to delete</param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Delete<T> (T entity, out HttpStatusCode code)
            where T : BaseEntity
        {
            string ignore;
            return Delete<T> (entity, out code, out ignore);
        }

        /// <summary>
        /// Delete an entity through the API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Entity to delete</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Delete<T> (T entity, out HttpStatusCode code, out string message)
            where T : BaseEntity
        {
            return Delete<T> (entity.IdToString, out code, out message);
        }

        /// <summary>
        /// Delete an entity by Entity ID.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">Entity ID</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Delete<T> (object id)
        {
            HttpStatusCode ignore;
            return Delete<T> (id, out ignore);
        }

        /// <summary>
        /// Delete an entity by Entity ID.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">Entity ID</param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Delete<T> (object id, out HttpStatusCode code)
        {
            string ignore;
            return Delete<T> (id, out code, out ignore);
        }

        /// <summary>
        /// Delete an entity by Entity ID.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">Entity ID</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool Delete<T> (object id, out HttpStatusCode code, out string message)
        {
            var response = AvoidAsyncDeadlock(() => DeleteAsync<T>(id)).Result;
            code = response.StatusCode;
            message = response.Content;

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public Task<MojioResponse<bool>> DeleteAsync<T> (object id)
        {
            string action = Map [typeof(T)];
            var request = GetRequest (Request (action, id), Method.DELETE);

            return RequestAsync<bool> (request);
        }

        /// <summary>
        /// Update/save an entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Updated Entity</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Update<T> (T entity)
            where T : BaseEntity, new()
        {
            HttpStatusCode ignore;
            return Update<T> (entity, out ignore);
        }

        /// <summary>
        /// Update/save an entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Updated Entity</param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Update<T> (T entity, out HttpStatusCode code)
            where T : BaseEntity, new()
        {
            string ignore;
            return Update<T> (entity, out code, out ignore);
        }

        /// <summary>
        /// Update/save an entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Updated Entity</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Update<T> (T entity, out HttpStatusCode code, out string message)
            where T : BaseEntity, new()
        {
            var response = AvoidAsyncDeadlock(() => UpdateAsync<T>(entity)).Result;
            code = response.StatusCode;
            message = response.Content;

            return response.Data;
        }

        public Task<MojioResponse<T>> UpdateAsync<T> (T entity)
            where T : BaseEntity, new()
        {
            string action = Map [typeof(T)];
            var request = GetRequest (Request (action, entity.IdToString), Method.PUT);
            request.AddBody (entity);

            return RequestAsync<T> (request);
        }

        /// <summary>
        /// Get an entity by ID.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">Entity ID</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Get<T> (object id)
            where T : new()
        {
            HttpStatusCode ignore;
            return Get<T> (id, out ignore);
        }

        /// <summary>
        /// Get an entity by ID.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">Entity ID</param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Get<T> (object id, out HttpStatusCode code)
            where T : new()
        {
            string ignore;
            return Get<T> (id, out code, out ignore);
        }

        /// <summary>
        /// Not so pretty fix for async deadlocks.  All synchronous calls are being deprecated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        private async Task<T> AvoidAsyncDeadlock<T> (Func<Task<T>> func)
        {
            return await Task.Factory.StartNew(() =>
            {
                var task = func();

                return task.Result;
            }).ConfigureAwait(continueOnCapturedContext: false);
        }

        /// <summary>
        /// Get an entity by ID.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">Entity ID</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public T Get<T> (object id, out HttpStatusCode code, out string message)
            where T : new()
        {
            var task = AvoidAsyncDeadlock(() => GetAsync<T>(id));
            task.Wait ();

            var response = task.Result;
            code = response.StatusCode;
            message = response.Content;

            return response.Data;
        }

        public Task<MojioResponse<T>> GetAsync<T> (object id)
            where T : new()
        {
            string action = Map [typeof(T)];
            var request = GetRequest (Request (action, id), Method.GET);

            return RequestAsync<T> (request);
        }

        /// <summary>
        /// Get a collection of M entities associated to a particular entity.
        /// </summary>
        /// <typeparam name="M">Entity type to fetch</typeparam>
        /// <typeparam name="T">Entity type to search by</typeparam>
        /// <param name="entity">Entity to search by</param>
        /// <param name="page">Pagenation page</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<M> GetBy<M, T> (T entity, int page = 1)
            where T : BaseEntity, new()
        {
            HttpStatusCode ignore;
            return GetBy<M, T> (entity, out ignore, page);
        }

        /// <summary>
        /// Get a collection of M entities associated to a particular entity.
        /// </summary>
        /// <typeparam name="M">Entity type to fetch</typeparam>
        /// <typeparam name="T">Entity type to search by</typeparam>
        /// <param name="entity">Entity to search by</param>
        /// <param name="code">Response code</param>
        /// <param name="page">Pagenation page</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<M> GetBy<M, T> (T entity, out HttpStatusCode code, int page = 1)
            where T : BaseEntity, new()
        {
            string ignore;
            return GetBy<M, T> (entity, out code, out ignore, page);
        }

        /// <summary>
        /// Get a collection of M entities associated to a particular entity.
        /// </summary>
        /// <typeparam name="M">Entity type to fetch</typeparam>
        /// <typeparam name="T">Entity type to search by</typeparam>
        /// <param name="entity">Entity to search by</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <param name="page">Pagenation page</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<M> GetBy<M, T> (T entity, out HttpStatusCode code, out string message, int page = 1)
            where T : BaseEntity, new()
        {
            return GetBy<M, T> (entity.IdToString, out code, out message, page);
        }

        /// <summary>
        /// Get a collection of M entities associated to a particular entity.
        /// </summary>
        /// <typeparam name="M">Entity type to fetch</typeparam>
        /// <typeparam name="T">Entity type to search by</typeparam>
        /// <param name="id">Id of entity to search by</param>
        /// <param name="action">Specific action name to call</param>
        /// <param name="page">Pagenation page</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<M> GetBy<M, T> (object id, int page = 1, string action = null)
            where T : new()
        {
            HttpStatusCode ignore;
            return GetBy<M, T> (id, out ignore, page, action);
        }

        /// <summary>
        /// Get a collection of M entities associated to a particular entity.
        /// </summary>
        /// <typeparam name="M">Entity type to fetch</typeparam>
        /// <typeparam name="T">Entity type to search by</typeparam>
        /// <param name="id">Id of entity to search by</param>
        /// <param name="code">Response code</param>
        /// <param name="action">Specific action name to call</param>
        /// <param name="page">Pagenation page</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<M> GetBy<M, T> (object id, out HttpStatusCode code, int page = 1, string action = null)
            where T : new()
        {
            string ignore;
            return GetBy<M, T> (id, out code, out ignore, page, action);
        }

        /// <summary>
        /// Get a collection of M entities associated to a particular entity.
        /// </summary>
        /// <typeparam name="M">Entity type to fetch</typeparam>
        /// <typeparam name="T">Entity type to search by</typeparam>
        /// <param name="id">Id of entity to search by</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <param name="action">Specific action name to call</param>
        /// <param name="page">Pagenation page</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<M> GetBy<M,T> (object id, out HttpStatusCode code, out string message, int page = 1, string action = null)
            where T : new()
        {
            var response = AvoidAsyncDeadlock(() => GetByAsync<M, T>(id, page, action)).Result;
            code = response.StatusCode;
            message = response.Content;

            return response.Data;
        }

        public Task<MojioResponse<Results<M>>> GetByAsync<M, T> (object id, int page = 1, string action = null)
            where T : new()
        {
            string controller = Map [typeof(T)];

            if (action == null)
                action = Map [typeof(M)];

            var request = GetRequest (Request (controller, id, action), Method.GET);

            request.AddParameter ("offset", Math.Max (0, (page - 1)) * PageSize);
            request.AddParameter ("limit", PageSize);

            return RequestAsync<Results<M>> (request);
        }

        /// <summary>
        /// Get a collection of entities available to the current session.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="page">Page</param>
        /// <param name="sortBy">Sort By</param>
        /// <param name="desc">Descending?</param>
        /// <param name="criteria">Criteria</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<T> Get<T>(int page = 1, Expression<Func<T, object>> sortBy = null, bool desc = false, string criteria = null)
            where T : new()
        {
            HttpStatusCode ignore;
            return Get<T> (out ignore, page, sortBy, desc, criteria);
        }

        /// <summary>
        /// Get a collection of entities available to the current session.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="code">Response code</param>
        /// <param name="page">Page</param>
        /// <param name="sortBy">Sort By</param>
        /// <param name="desc">Descending?</param>
        /// <param name="criteria">Criteria</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<T> Get<T>(out HttpStatusCode code, int page = 1, Expression<Func<T, object>> sortBy = null, bool desc = false, string criteria = null)
            where T : new()
        {
            string ignore;
            return Get<T>(out code, out ignore, page, sortBy, desc, criteria);
        }

        /// <summary>
        /// Get a collection of entities available to the current session.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <param name="page">Page</param>
        /// <param name="sortBy">Sort By</param>
        /// <param name="desc">Descending?</param>
        /// <param name="criteria">Criteria</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public Results<T> Get<T>(out HttpStatusCode code, out string message, int page = 1, Expression<Func<T, object>> sortBy = null, bool desc = false, string criteria = null)
            where T : new()
        {
            var response = AvoidAsyncDeadlock(() => GetAsync<T>(page, sortBy, desc, criteria)).Result;
            code = response.StatusCode;
            message = response.Content;

            return response.Data;
        }

        public Task<MojioResponse<Results<T>>> GetAsync<T>(int page = 1, Expression<Func<T, object>> sortBy = null, bool desc = false, string criteria = null)
            where T : new()
        {
            string action = Map [typeof(T)];
            var request = GetRequest (Request (action), Method.GET);

            request.AddParameter("offset", Math.Max (0, (page - 1)) * PageSize);
            request.AddParameter("limit", PageSize);
            if (sortBy != null)
            {
                var expr = GetMemberInfo(sortBy);
                request.AddParameter("sortBy", expr.Member.Name);

            }
            request.AddParameter("desc", desc);
            if (!string.IsNullOrWhiteSpace(criteria))
                request.AddParameter ("criteria", criteria);

            return RequestAsync<Results<T>> (request);
        }

        public void ThrowError (string errorMessage)
        {
            throw new Exception (errorMessage);
        }

        public static T Deserialize<T> (string content)
        {
            var serializer = new RSJsonSerializer ();
            return serializer.Deserialize<T> (content);
        }

        public IMojioQueryable<T> Queryable<T> ()
            where T : BaseEntity, new()
        {
            string action = Map [typeof(T)];
            var request = GetRequest (Request (action), Method.GET);

            var provider = new MojioQueryProvider<T> (this, Request (action));
            return new MojioQueryable<T> (provider);
        }

        /// <summary>
        /// Clear all subscriptions for specific channel and target
        /// </summary>
        /// <param name="channel">Channel type</param>
        /// <param name="target">target string</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ClearSubscriptions (ChannelType channel, String target)
        {
            HttpStatusCode ignore;
            return ClearSubscriptions (channel, target, out ignore);
        }

        /// <summary>
        /// Clear all subscriptions for specific channel and target
        /// </summary>
        /// <param name="channel">Channel type</param>
        /// <param name="target">target string</param>
        /// <param name="code">Response code</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ClearSubscriptions (ChannelType channel, String target, out HttpStatusCode code)
        {
            string ignore;
            return ClearSubscriptions (channel, target, out code, out ignore);
        }

        /// <summary>
        /// Clear all subscriptions for specific channel and target
        /// </summary>
        /// <param name="channel">Channel type</param>
        /// <param name="target">target string</param>
        /// <param name="code">Response code</param>
        /// <param name="message">Response message</param>
        /// <returns></returns>
        [Obsolete("Synchronous are deprecated, please use Async metho instead.")]
        public bool ClearSubscriptions (ChannelType channel, String target, out HttpStatusCode code, out string message)
        {
            var response = AvoidAsyncDeadlock(() => ClearSubscriptionsAsync(channel, target)).Result;
            code = response.StatusCode;
            message = response.Content;

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Clear all subscriptions for specific channel and target
        /// </summary>
        /// <param name="channel">Channel type</param>
        /// <param name="target">target string</param>
        /// <returns></returns>
        public Task<MojioResponse> ClearSubscriptionsAsync (ChannelType channel, String target)
        {
            string action = Map [typeof(Subscription)];
            var request = GetRequest (Request (action), Method.DELETE);
            request.AddParameter ("channel", channel);
            request.AddParameter ("target", target);

            return RequestAsync (request);
        }

        MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null)
                throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr =
                    ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");

            return memberExpr;
        }
    }
}
