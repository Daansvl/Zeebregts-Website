using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.WebServices.Data;
using System.Net;

namespace zeebregtsCs.ExchangeComs
{
    public static class EWSFunctions
    {


        public static Microsoft.Exchange.WebServices.Data.ExchangeService GetNewServiceHook()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            Microsoft.Exchange.WebServices.Data.ExchangeService service = new Microsoft.Exchange.WebServices.Data.ExchangeService(ExchangeVersion.Exchange2013_SP1);
            service.Credentials = new WebCredentials("joost@zeebregts.nl", "Pallas18#2015");
            service.UseDefaultCredentials = false;

            service.TraceEnabled = false;
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            return service;
        }

        public static void MakePublicContactsFolder(Microsoft.Exchange.WebServices.Data.ExchangeService service)
        {
            var folder = new ContactsFolder(service);
            folder.DisplayName = "ZeebregtsCs";
            folder.Save(WellKnownFolderName.Contacts);

        }
        public static void SendEmail(Microsoft.Exchange.WebServices.Data.ExchangeService service)
        {
            EmailMessage email = new EmailMessage(service);
            email.ToRecipients.Add("daan@zeebregts.nl");
            email.Subject = "HelloWorld";
            email.Body = new MessageBody("This is the first email I've sent by using the EWS Managed API.");
            email.Send();
        }

        private static PhoneNumberKey ConvertTelTypes(int typenr)
        {
            var numberkey = PhoneNumberKey.PrimaryPhone;
            switch (typenr)
            {
                case 0:
                    numberkey = PhoneNumberKey.PrimaryPhone;
                    break; //"vast"
                case 1:
                    numberkey = PhoneNumberKey.MobilePhone;
                    break;
                case 2:
                    numberkey = PhoneNumberKey.BusinessFax;
                    break;
                case 3:
                    numberkey = PhoneNumberKey.OtherTelephone;
                    break;
                case 4:
                    numberkey = PhoneNumberKey.Isdn;
                    break;
                case 5:
                    numberkey = PhoneNumberKey.CompanyMainPhone;
                    break;
                case 6:
                    numberkey = PhoneNumberKey.HomePhone2;
                    break;
                case 7:
                    numberkey = PhoneNumberKey.BusinessPhone2;
                    break;
            }
            return numberkey;
        }
        public static void MakeNewContact(Microsoft.Exchange.WebServices.Data.ExchangeService service,ExchangeContactItem contactinfo)
        {
            var f_id = FindFolder(service);
            Contact contact = new Contact(service);
            Console.WriteLine("Making new contact: " + contactinfo.Achternaam + "," + contactinfo.Voornaam);
            // Specify the name and how the contact should be filed.
            contact.GivenName = String.IsNullOrEmpty(contactinfo.Voornaam) == false ? contactinfo.Voornaam : "" ;
            contact.MiddleName = String.IsNullOrEmpty(contactinfo.Tussenvoegsel) == false ? contactinfo.Tussenvoegsel : "";
            contact.Surname = String.IsNullOrEmpty(contactinfo.Achternaam) == false ? contactinfo.Achternaam : "";
            contact.FileAsMapping = FileAsMapping.SurnameCommaGivenName;

            // Specify the company name.
            contact.CompanyName = String.IsNullOrEmpty(contactinfo.BedrijfNaam) == false ? contactinfo.BedrijfNaam : "";

            // Specify the business, home, and car phone numbers.
            var phone1type = PhoneNumberKey.PrimaryPhone;
            var phone2type = PhoneNumberKey.MobilePhone;
            var phone3type = PhoneNumberKey.BusinessPhone;
            if(!String.IsNullOrEmpty(contactinfo.TelNrTypes))
            {
                var sets = contactinfo.TelNrTypes.Split(',');
                var nr1 = int.Parse(sets[0].Substring(0,1));
                var nr2 = int.Parse(sets[1].Substring(0,1));
                var nr3 = int.Parse(sets[2].Substring(0,1));
                if(nr2 == nr1)
                {
                    nr2 = 6;
                }
                if(nr3 == nr2 || nr3 == nr1)
                {
                    nr3 = 7;
                }

                phone1type = ConvertTelTypes(nr1);
                phone2type = ConvertTelTypes(nr2);
                phone3type = ConvertTelTypes(nr3);
            }

            contact.PhoneNumbers[phone1type] = String.IsNullOrEmpty(contactinfo.TelNr1) == false ? contactinfo.TelNr1 : "";
            contact.PhoneNumbers[phone2type] = String.IsNullOrEmpty(contactinfo.TelNr2) == false ? contactinfo.TelNr2 : "";
            contact.PhoneNumbers[phone3type] = String.IsNullOrEmpty(contactinfo.TelNr3) == false ? contactinfo.TelNr3 : "";

            // Specify email addresses.
            contact.EmailAddresses[EmailAddressKey.EmailAddress1] = new EmailAddress(String.IsNullOrEmpty(contactinfo.Email1) == false ? contactinfo.Email1 : "niet@bekend.nl");
            

            //functie
            contact.JobTitle = String.IsNullOrEmpty(contactinfo.Functie) == false ? contactinfo.Functie : "";
            // Save the contact.
            contact.Save(f_id);

        }

      
        public static void DeleteContact(Microsoft.Exchange.WebServices.Data.ExchangeService service, ItemId contactUniId)
        {
            Console.WriteLine("Deleting contact");
            Contact contact = Contact.Bind(service, contactUniId);
            contact.Delete(DeleteMode.MoveToDeletedItems);
        }

        public static void UpdateContact(Microsoft.Exchange.WebServices.Data.ExchangeService service, ItemId contactUniId,ExchangeContactItem contactinfo)
        {
            // Bind to an existing meeting request by using its unique identifier.
            Contact contact = Contact.Bind(service, contactUniId);

            // Specify the name and how the contact should be filed.
            if(String.IsNullOrEmpty(contactinfo.Voornaam) == false && contact.GivenName != contactinfo.Voornaam)
            {
                contact.GivenName = contactinfo.Voornaam;
            }
            if(String.IsNullOrEmpty(contactinfo.Tussenvoegsel) == false && contact.MiddleName != contactinfo.Tussenvoegsel)
            {
                contact.MiddleName =  contactinfo.Tussenvoegsel;
            }
            if(String.IsNullOrEmpty(contactinfo.Achternaam) == false && contactinfo.Achternaam != contact.Surname)
            {
                contact.Surname = contactinfo.Achternaam;
            }
            
            // Specify the company name.
            if(String.IsNullOrEmpty(contactinfo.BedrijfNaam) == false &&  contact.CompanyName != contactinfo.BedrijfNaam)
            {
                contact.CompanyName = contactinfo.BedrijfNaam;
            }
            

            // Specify the phone numbers.

            var phone1type = PhoneNumberKey.PrimaryPhone;
            var phone2type = PhoneNumberKey.MobilePhone;
            var phone3type = PhoneNumberKey.BusinessPhone;
            if (!String.IsNullOrEmpty(contactinfo.TelNrTypes))
            {
                var sets = contactinfo.TelNrTypes.Split(',');
                var nr1 = int.Parse(sets[0]);
                var nr2 = int.Parse(sets[1]);
                var nr3 = int.Parse(sets[2]);
                if (nr2 == nr1)
                {
                    nr2 = 6;
                }
                if (nr3 == nr2 || nr3 == nr1)
                {
                    nr3 = 7;
                }

                phone1type = ConvertTelTypes(nr1);
                phone2type = ConvertTelTypes(nr2);
                phone3type = ConvertTelTypes(nr3);
                if (phone1type == PhoneNumberKey.BusinessFax)
                {
                    contactinfo.TelNr1 = "Fax:" + contactinfo.TelNr1;
                    phone1type = PhoneNumberKey.Pager;
                }
                if (phone2type == PhoneNumberKey.BusinessFax)
                {
                    contactinfo.TelNr2 = "Fax:" + contactinfo.TelNr2;
                    phone2type = PhoneNumberKey.Pager;
                }

                if (phone3type == PhoneNumberKey.BusinessFax)
                {
                    contactinfo.TelNr3 = "Fax:" + contactinfo.TelNr3;
                    phone3type = PhoneNumberKey.Pager;
                }

            }
            
            if (String.IsNullOrEmpty(contactinfo.TelNr1) == false)
            {
                if (contact.PhoneNumbers.Contains(phone1type) == true)
                {
                    if(contact.PhoneNumbers[phone1type] != contactinfo.TelNr1)
                    {
                        contact.PhoneNumbers[phone1type] = contactinfo.TelNr1;
                    }
                }
                else
                {
                    contact.PhoneNumbers[phone1type] = contactinfo.TelNr1;
                }
              
            }
            if (String.IsNullOrEmpty(contactinfo.TelNr2) == false)
            {
                if (contact.PhoneNumbers.Contains(phone2type) == true)
                {
                    if (contact.PhoneNumbers[phone2type] != contactinfo.TelNr2)
                    {
                        contact.PhoneNumbers[phone2type] = contactinfo.TelNr2;
                    }
                }
                else
                {
                    contact.PhoneNumbers[phone2type] = contactinfo.TelNr2;
                }

            }
            if (String.IsNullOrEmpty(contactinfo.TelNr3) == false)
            {
                if (contact.PhoneNumbers.Contains(phone3type) == true)
                {
                    if (contact.PhoneNumbers[phone3type] != contactinfo.TelNr3)
                    {
                        contact.PhoneNumbers[phone3type] = contactinfo.TelNr3;
                    }
                }
                else
                {
                    contact.PhoneNumbers[phone3type] = contactinfo.TelNr3;
                }

            }
            
           
            // Specify two email addresses.
            contact.EmailAddresses[EmailAddressKey.EmailAddress1] = new EmailAddress(String.IsNullOrEmpty(contactinfo.Email1) == false ? contactinfo.Email1 : "niet@bekend.nl");

            //save the changes
            contact.Update(ConflictResolutionMode.AlwaysOverwrite);
           
        }

        //check if return itemid != "0"
        public static ItemId FindContact(Microsoft.Exchange.WebServices.Data.ExchangeService service, string achternaam,string voornaam,string tussenvoegsel )
        {
            var returnID = new ItemId("0");
            var folders = new List<FolderId>(1);
            var f_id = FindFolder(service);

            // Only use the Contacts folder.
            //NameResolutionCollection resolvedNames = service.ResolveName(surnameCommaGivenname,folders , ResolveNameSearchLocation.ContactsOnly, true);
            var contactView = new ItemView(1);
            var filterCol = new SearchFilter.SearchFilterCollection();
            if (!String.IsNullOrEmpty(achternaam))
            {
                var filterSurname = new SearchFilter.IsEqualTo(ContactSchema.Surname, achternaam);
                filterCol.Add(filterSurname);
            }
            if (!String.IsNullOrEmpty(voornaam))
            {
                var filterGivenName = new SearchFilter.IsEqualTo(ContactSchema.GivenName, voornaam);
                filterCol.Add(filterGivenName);
            }
            if(!String.IsNullOrEmpty(tussenvoegsel))
            {
                var filterTussenvoegsel = new SearchFilter.IsEqualTo(ContactSchema.MiddleName, tussenvoegsel);
                filterCol.Add(filterTussenvoegsel);
            }
            var contacts = service.FindItems(f_id, filterCol, contactView);

            if (contacts != null && contacts.Count() > 0 && contacts.First() != null)
            {
                Console.WriteLine("Contact Found");
                returnID = (contacts.First() as Contact).Id;
            }

            return returnID;
        }

        public static FolderId FindFolder(Microsoft.Exchange.WebServices.Data.ExchangeService service)
        {
            var resultid = new FolderId("0");
            var responseview = new FolderView(1);
            var filter = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "sync-adressen");

            var folderresponse = service.FindFolders(WellKnownFolderName.PublicFoldersRoot, filter, responseview);

            if (folderresponse.Count() == 1)
            {
                var responseview2 = new FolderView(1);
                var filter2 = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "sync-contacten");
                var folderresponse2 = service.FindFolders(folderresponse.First().Id, filter2, responseview2);
                if (folderresponse2.Count() == 1)
                {
                    var responseview3 = new FolderView(1);
                    var filter3 = new SearchFilter.IsEqualTo(FolderSchema.DisplayName, "syncContacts");
                    var folderresponse3 = service.FindFolders(folderresponse2.First().Id, filter3, responseview3);
                    if (folderresponse3.Count() == 1)
                    {
                        resultid = folderresponse3.First().Id;
                    }
                }
            }
            return resultid;
        }

       public static void MakeFolder(Microsoft.Exchange.WebServices.Data.ExchangeService service)
        {
           ContactsFolder folder = new ContactsFolder(service);
           folder.DisplayName = "syncContacts";
           folder.Save(FindFolder(service));
        }
    }
}
