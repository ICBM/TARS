using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Net;

namespace TARS.Helpers
{
    public class LDAPConnection
    {
        public List<string> create(string addUser)
        {
            var properties = new List<string>();

            //Create LDAP Connection object
            DirectoryEntry myLdapConnection = createDirectoryEntry();

            //Create search object which operates on LDAP connection object
            // and set search object to only find the user specified
            DirectorySearcher search = new DirectorySearcher(myLdapConnection);
            search.Filter = "(cn=" + addUser + ")";

            // create results objects from search object
            SearchResult result = search.FindOne();

            if (result != null)
            {
                // user exists, cycle through LDAP fields; in final 
                //  application, we will only select those that are
                //  needed for the TARSUser model; for now, it just
                //  pulls all of them
                ResultPropertyCollection fields = result.Properties;

                foreach (string ldapField in fields.PropertyNames)
                {
                    foreach (Object attr in fields[ldapField])
                    {
                        properties.Add(String.Format("{0,-20} : {1}", ldapField, attr.ToString()));
                    }
                }
            }

            return properties;
        }

        static DirectoryEntry createDirectoryEntry()
        {
            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://dhw.state.id.us/dc=dhw,dc=state,dc=id,dc=us");
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;

            return ldapConnection;
        }
    }
}
