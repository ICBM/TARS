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
        //this standard of uid=admin,ou=system IS EXTREMELY IMPORTANT. Not having this when we try to Bind() will throw "Syntax Error" exceptions
        //would have been nice if someone told me that before I spent WAY too much time on Google trying to figure this out
        private static string user = "uid=admin,ou=system"; //we'll re-use these within the class. make it easily modifiable for the users who come after us
        private static string pw = "secret";
        private static string domain = "tars.com"; //built for local server. change to whatever is relevent
        private static string port = ":10389"; //default port for ActiveDirectory LDAP is 389. ApacheDS uses 10389
        private static string targetOU = "O=system,DC=example,DC=com"; //default

        private NetworkCredential credential; //one credential to go with the one connection; built using user + pw + domain
        private LdapDirectoryIdentifier identifier; //built using domain + port
        private LdapConnection connection; //one connection to each LDAPConnection class instance; built using identifier

        private string dn;


        public LDAPConnection()
        {
            //connection = new LdapConnection(domain);
            credential = new NetworkCredential(user, pw);
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
            credential = new NetworkCredential(user, pw, domain); //extremely important, must follow this exact pattern to work with apacheDS
            identifier = new LdapDirectoryIdentifier(domain + port, true, false);

        }

        //if this returns true, go ahead and add the model.user to the cookie for login
        public bool requestUser(string user, string password)
        {
            string ldapSearchFilters = "(objectClass=*)"; //required. else we get a compilation error and explode
            dn = "cn=" + user + ",ou=users,o=tars";

            //connection = new LdapConnection(identifier); //start up our connection

            try
            {

                connection = new LdapConnection(identifier, credential); //start up our connection
                
                connection.Credential = credential; //not sure if this is necessary, not changing it
                connection.AuthType = AuthType.Basic; //absolutely necessary, otherwise we can't Bind()
                connection.SessionOptions.ProtocolVersion = 3; //also necessary, some wonky version problems
                connection.Bind(); //actually bind to the server so we can make some queries!

                System.Diagnostics.Debug.WriteLine("LdapConnection is created successfully.");
               
                SearchRequest searchRequest = new SearchRequest
                                                (dn,
                                                  ldapSearchFilters,
                                                  System.DirectoryServices.Protocols.SearchScope.Subtree,
                                                  null);

                // cast the returned directory response as a SearchResponse object
                SearchResponse searchResponse =
                            (SearchResponse)connection.SendRequest(searchRequest);

                Console.WriteLine("\r\nSearch Response Entries:{0}",
                            searchResponse.Entries.Count);

                // enumerate the entries in the search response
                foreach (SearchResultEntry entry in searchResponse.Entries)
                {
                    System.Diagnostics.Debug.WriteLine("{0}:{1}",
                        searchResponse.Entries.IndexOf(entry),
                        entry.DistinguishedName);
                }
           
            
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("\nUnexpected exception occured:\n\t{0}: {1}: {2}",
                                   e.GetType().Name, e.Message, e.ToString());
                return false;
            }


            connection.Dispose();
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