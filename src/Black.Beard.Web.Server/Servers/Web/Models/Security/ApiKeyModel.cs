namespace Bb.Servers.Web.Models.Security
{

    public class ApiKeyModel
    {

        public ApiKeyModel()
        {
            Contracts = new List<string>();
            Claims = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Owner of the key
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Key to prover access
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Key is admin
        /// </summary>
        public bool Admin { get; set; }

        /// <summary>
        /// Contracts
        /// </summary>
        public List<string> Contracts { get; set; }

        /// <summary>
        /// Claims
        /// </summary>
        public List<KeyValuePair<string, string>> Claims { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ApiKey CreateFrom()
        {

            var m = new ApiKey()
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Key = Key,
                Owner = Owner,
                Admin = Admin
            };

            m.Contracts.AddRange(Contracts);
            m.Claims.AddRange(Claims);

            return m;

        }


    }


}
