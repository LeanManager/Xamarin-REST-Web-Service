using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BookClient.Data
{
	/* 
	   The service requires you "login" first to get a token. There is no user authentication 
       (i.e. you do not have to enter any credentials); however, you must call a specific endpoint 
       first to get a token. You must then send the token back to the server on each subsequent request in the HTTP header.
    */

	public class BookManager
    {
        const string Url = "http://xam150.azurewebsites.net/api/books/";

        private string authorizationKey;


        private async Task<HttpClient> GetClient()
        {
            var client = new HttpClient();

			/* 
			   If this is the first time the method has been called, then the authorizationKey field will not be set. 
               In this case, you need to use GetStringAsync with the base URL + login to get the token
            */
			if (string.IsNullOrEmpty(authorizationKey))
			{
				authorizationKey = await client.GetStringAsync(Url + "login");

				// The returned token will have quotes around it which need to be removed.
				authorizationKey = JsonConvert.DeserializeObject<string>(authorizationKey);
			}

			// Add an Authorization header to the DefaultRequestHeaders collection of the HttpClient. Use the token as the value.
			client.DefaultRequestHeaders.Add("Authorization", authorizationKey);

			// Add an Accept header to the DefaultRequestHeaders collection of the HttpClient. Use application/json as the value.
			client.DefaultRequestHeaders.Add("Accept", "application/json");

			return client;
        }


        public async Task<IEnumerable<Book>> GetAll()
        {
			// TODO: use GET to retrieve books - SELECT

			// Use your GetClient method to retrieve an HttpClient to work with.
			HttpClient client = await GetClient();

			// Use GetStringAsync on the base Url to retrieve the array of books.
			string result = await client.GetStringAsync(Url);

			// Pass the returned string into JsonConvert.DeserializeObject to turn 
            // the JSON data into an object graph and return it back to the caller.
			return JsonConvert.DeserializeObject<IEnumerable<Book>>(result);
        }


        public async Task<Book> Add(string title, string author, string genre)
        {
            // TODO: use POST to add a book - INSERT INTO

            Book book = new Book()
            {
                ISBN = string.Empty,
                Title = title,
                Authors = new List<string>(new[] { author }),
                PublishDate = DateTime.Now.Date,
                Genre = genre
            };

			HttpClient client = await GetClient();

			// Use the PostAsync method against the base URL to add the book.
			// Create the HttpContent from the JSON string by creating a new StringContent object, 
            // use the constructor which also takes an encoding and media type.
			var response = await client.PostAsync(Url, new StringContent(JsonConvert.SerializeObject(book),
					                                                     Encoding.UTF8, "application/json"));

			return JsonConvert.DeserializeObject<Book>(await response.Content.ReadAsStringAsync());
        }


        public async Task Update(Book book)
        {
            // TODO: use PUT to update a book - UPDATE

            HttpClient client = await GetClient();

            await client.PutAsync(Url + book.ISBN, new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json"));
        }


        public async Task Delete(string isbn)
        {
            // TODO: use DELETE to delete a book - DELETE

            HttpClient client = await GetClient();

            await client.DeleteAsync(Url + isbn);
        }
    }
}

