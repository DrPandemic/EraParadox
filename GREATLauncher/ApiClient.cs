using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GREATLauncher
{
    public class ApiClient
    {
        private const string BASE_URI = "http://api.eraparadox.com/api/v1/";

        public class User
        {
            public int id { get; set; }
            public string email { get; set; }
            public string username { get; set; }
            public bool admin { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }

            public override string ToString()
            {
                return this.username;
            }
        }

        public class Post
        {
            public int id { get; set; }
            public string title { get; set; }
            public string slug { get; set; }
            public string content { get; set; }
            public int user_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }

            public override string ToString()
            {
                return this.title;
            }
        }

        public class Server
        {
            public int id { get; set; }
            public string host { get; set; }
            public int port { get; set; }
            public string status { get; set; }
            public int game_id { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }

            public override string ToString()
            {
                return this.host + ":" + this.port;
            }
        }

        public class Game
        {
            public int id { get; set; }
            public string status { get; set; }
            public int player_count { get; set; }
            public int force_start { get; set; }
            public List<User> users { get; set; }
            public Server server { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        private class TokenAuthenticator : IAuthenticator
        {
            private string token;

            public TokenAuthenticator(string token)
            {
                this.token = token;
            }

            public void Authenticate(IRestClient client, IRestRequest request)
            {
                request.AddParameter("auth_token", this.token, ParameterType.QueryString);
            }
        }

        private RestClient client = new RestClient(BASE_URI);
        private string token;

        public Task<bool> SignIn(string email, string password)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            RestRequest req = new RestRequest("sessions", Method.POST);
            req.AddParameter("email", email);
            req.AddParameter("password", password);

            this.client.ExecuteAsync<Dictionary<string, object>>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    this.token = resp.Data["token"].ToString();
                    this.client.Authenticator = new TokenAuthenticator(this.token);
                    tcs.SetResult(true);
                } else {
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }

        public Task<bool> SignOut()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            RestRequest req = new RestRequest("sessions/{id}", Method.DELETE);
            req.AddUrlSegment("id", this.token);

            this.client.ExecuteAsync(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    this.token = null;
                    this.client.Authenticator = null;
                    tcs.SetResult(true);
                } else {
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }

        public Task<User> GetUser()
        {
            TaskCompletionSource<User> tcs = new TaskCompletionSource<User>();

            RestRequest req = new RestRequest("users", Method.GET);

            this.client.ExecuteAsync<User>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        public Task<User> GetUser(int id)
        {
            TaskCompletionSource<User> tcs = new TaskCompletionSource<User>();

            RestRequest req = new RestRequest("users/{id}", Method.GET);
            req.AddUrlSegment("id", id.ToString());

            this.client.ExecuteAsync<User>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        public Task<List<Post>> GetPosts()
        {
            TaskCompletionSource<List<Post>> tcs = new TaskCompletionSource<List<Post>>();

            RestRequest req = new RestRequest("posts", Method.GET);

            this.client.ExecuteAsync<List<Post>>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        public Task<Post> GetPost(int id)
        {
            TaskCompletionSource<Post> tcs = new TaskCompletionSource<Post>();

            RestRequest req = new RestRequest("posts/{id}", Method.GET);
            req.AddUrlSegment("id", id.ToString());

            this.client.ExecuteAsync<Post>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        public Task<List<User>> GetFriends()
        {
            TaskCompletionSource<List<User>> tcs = new TaskCompletionSource<List<User>>();

            RestRequest req = new RestRequest("friends", Method.GET);

            this.client.ExecuteAsync<List<User>>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        public Task<bool> CreateFriend(string username)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            RestRequest req = new RestRequest("friends", Method.POST);
            req.AddParameter("username", username);

            this.client.ExecuteAsync(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(true);
                } else {
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }

        public Task<bool> DestroyFriend(int id)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            RestRequest req = new RestRequest("friends/{id}", Method.DELETE);
            req.AddUrlSegment("id", id.ToString());

            this.client.ExecuteAsync(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(true);
                } else {
                    tcs.SetResult(false);
                }
            });

            return tcs.Task;
        }

        public Task<Game> GetGame()
        {
            TaskCompletionSource<Game> tcs = new TaskCompletionSource<Game>();

            RestRequest req = new RestRequest("games", Method.GET);

            this.client.ExecuteAsync<Game>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        public Task<Game> GetGame(int id)
        {
            TaskCompletionSource<Game> tcs = new TaskCompletionSource<Game>();

            RestRequest req = new RestRequest("games/{id}", Method.GET);
            req.AddUrlSegment("id", id.ToString());

            this.client.ExecuteAsync<Game>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }

        public Task<Game> UpdateGame(int id, int vote)
        {
            TaskCompletionSource<Game> tcs = new TaskCompletionSource<Game>();

            RestRequest req = new RestRequest("games/{id}", Method.PUT);
            req.AddUrlSegment("id", id.ToString());
            req.AddParameter("vote", vote.ToString());

            this.client.ExecuteAsync<Game>(req, resp => {
                if (resp.StatusCode == HttpStatusCode.OK) {
                    tcs.SetResult(resp.Data);
                } else {
                    tcs.SetResult(null);
                }
            });

            return tcs.Task;
        }
    }
}
