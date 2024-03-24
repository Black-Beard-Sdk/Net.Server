namespace Bb.Servers.Web.Models.Security
{


    public class ApiItem
    {

        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Owner of the key
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Key to pass in the header
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Contracts
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// it is the admin key
        /// </summary>
        public bool Admin { get; set; }


    }


}
