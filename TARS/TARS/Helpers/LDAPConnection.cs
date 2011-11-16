using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;
using System.Net;

namespace TARS.Helpers
{
    public class LDAPConnection
    {
        private int exists = 0; //if the user exists, this will be 1. ensures that requestUser() is called before requestRole()  
        private static string user = "admin"; //we'll re-use these within the class. make it easily modifiable for the users who come after us
        private static string pw = "password";
        private static string domain = "localhost"; //built for local server. change to whatever is relevent
        private static string port = ":10389"; //default port for ActiveDirectory LDAP is 389. ApacheDS uses 10389

        private NetworkCredential credential; //one credential to go with the one connection; built using user + pw + domain
        private LdapDirectoryIdentifier identifier; //built using domain + port
        private LdapConnection connection; //one connection to each LDAPConnection class instance; built using identifier

        private string dn;


        public LDAPConnection()
        {
            //connection = new LdapConnection(domain);
            credential = new NetworkCredential(user, pw, domain);
            identifier = new LdapDirectoryIdentifier(domain + port);

        }

        //if you really want to, go ahead and specify your specific information
        public LDAPConnection(string specifiedUser, string specifiedPw, string specifiedDomain, string specifiedPort)
        {
            user = specifiedUser;
            pw = specifiedPw;
            domain = specifiedDomain;
            port = specifiedPort;

            //connection = new LdapConnection(domain);
            credential = new NetworkCredential(user, pw, domain);
            identifier = new LdapDirectoryIdentifier(domain + port);

        }

        //if this returns true, go ahead and add the model.user to the cookie for login
        public bool requestUser(string user, string password)
        {
            string ldapSearchFilters = "(objectClass=*)"; //required. else we get a compilation error and explode
            dn = "cn=" + user + "ou=users,ou=system";

            connection = new LdapConnection(identifier); //start up our connection

            try
            {
                SearchRequest searchRequest = new SearchRequest
                                                (dn,
                                                  ldapSearchFilters,
                                                  System.DirectoryServices.Protocols.SearchScope.OneLevel,
                                                  null);

                // cast the returned directory response as a SearchResponse object
                SearchResponse searchResponse =
                            (SearchResponse)connection.SendRequest(searchRequest);

                Console.WriteLine("\r\nSearch Response Entries:{0}",
                            searchResponse.Entries.Count);

                // enumerate the entries in the search response
                foreach (SearchResultEntry entry in searchResponse.Entries)
                {
                    Console.WriteLine("{0}:{1}",
                        searchResponse.Entries.IndexOf(entry),
                        entry.DistinguishedName);
                }
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("\nUnexpected exception occured:\n\t{0}: {1}",
                                   e.GetType().Name, e.Message);
                return false;
            }


            return true;
        }

        //return "User", "Manager", or "Admin" depending on the role
        public string requestRole(string user)
        {
            if (exists == 1)
            {
                return "User";
            }

            else return "None";
        }


        public string[] requestTasks(string user)
        {

            return null;
        }





    }
}