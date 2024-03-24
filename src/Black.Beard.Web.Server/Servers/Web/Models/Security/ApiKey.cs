namespace Bb.Servers.Web.Models.Security
{


    public class ApiKey
    {

        public ApiKey()
        {
            Contracts = new List<string>();
            Claims = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Owner of the key
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Key to provide access
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Date of creation
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Admin key
        /// </summary>
        public bool Admin { get; set; }

        /// <summary>
        /// Contracts
        /// </summary>
        public List<string> Contracts { get; set; }

        /// <summary>
        /// Claims of the api key
        /// </summary>
        public List<KeyValuePair<string, string>> Claims { get; set; }

        /// <summary>
        /// Set the admin key
        /// </summary>
        /// <param name="isAdmin"></param>
        /// <returns></returns>
        public ApiKey SetAdmin(bool isAdmin)
        {
            Admin = isAdmin;
            if (isAdmin)
            {
                Contracts.Clear();
            }
            return this;
        }

        /// <summary>
        /// Update the api key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ApiKey Update(ApiKeyModel data)
        {
            Contracts.Clear();
            Contracts.AddRange(data.Contracts);
            Claims.Clear();
            Claims.AddRange(data.Claims);
            Admin = data.Admin;
            return this;
        }

        /// <summary>
        /// Create a new api key
        /// </summary>
        /// <returns></returns>
        public ApiItem GetItemForList()
        {

            return new ApiItem()
            {
                Id = Id,
                Owner = Owner,
                Key = Key,
                Created = Created,
                Admin = Admin,
            };

        }


    }


}
