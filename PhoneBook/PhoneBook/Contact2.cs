using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook
{
    public class Contact2 : IComparable
    {
        private class SortAscendingHelper : IComparer
        {
            int IComparer.Compare(object? x, object? y)
            {
                if (x == null || y == null) throw new ArgumentException();

                Contact2 c1 = (Contact2)x;
                Contact2 c2 = (Contact2)y;

                if (c1.Favourite == c2.Favourite) return string.Compare(c1.DisplayName, c2.DisplayName);
                else if (c1.Favourite && !c2.Favourite) return -1;
                else return 1;
            }
        }
        private class SortDescendingHelper : IComparer
        {
            int IComparer.Compare(object? x, object? y)
            {
                if (x == null || y == null) throw new ArgumentException();

                Contact2 c1 = (Contact2)x;
                Contact2 c2 = (Contact2)y;

                if (c1.Favourite == c2.Favourite) return string.Compare(c1.DisplayName, c2.DisplayName);
                else if (c1.Favourite && !c2.Favourite) return 1;
                else return -1;
            }
        }

        public int Id { get => _id; }
        public string FirstName { get => _firstName; }
        public string LastName { get => _lastName; }
        public string DisplayName { get => $"{_firstName} {_lastName}"; }
        public string PhoneNumber { get => _phoneNumber; }
        public string EmailAddress { get => _emailAddress; }
        public string Company { get => _company; }
        public Image? Icon { get => _icon; }
        public bool Favourite { get => _favourite; }
        public  bool Updated { get => _updated; }
        public bool New { get => _new; }

        private int _id;
        private string _firstName, _lastName, _phoneNumber, _emailAddress, _company;
        private Image? _icon;
        private bool _favourite;
        private bool _updated = false, _new = false;

        public Contact2(object[] objects, bool isNew = false)
        {
            _id = (int)objects[0];
            _firstName = (string)objects[1];
            _lastName = !DBNull.Value.Equals(objects[2]) ? (string)objects[2] : "";
            _phoneNumber = !DBNull.Value.Equals(objects[3]) ? (string)objects[3] : "";
            _emailAddress = !DBNull.Value.Equals(objects[4]) ? (string)objects[4] : "";
            _company = !DBNull.Value.Equals(objects[5]) ? (string)objects[5] : "";
            _icon = !DBNull.Value.Equals(objects[6]) ? (Image)objects[6] : null;
            _favourite = (bool)objects[7];
            _new = isNew;
        }
        int IComparable.CompareTo(object? obj)
        {
            if (obj == null) throw new ArgumentException();

            Contact2 c = (Contact2)obj;

            if (this.Favourite == c.Favourite) return string.Compare(this.DisplayName, c.DisplayName);
            else if (this.Favourite && !c.Favourite) return 1;
            else return -1;
        }
        public static IComparer<Contact2>? SortAscending() { return (IComparer<Contact2>?) new SortAscendingHelper(); }
        public static IComparer<Contact2>? SortDescending() { return (IComparer<Contact2>?) new SortDescendingHelper(); }

        public void Update(int? id=null, string? firstName=null, string? lastName=null, string? phoneNumber=null, string? emailAddress=null, string? company=null, Image? icon=null, bool? favourite=null)
        {
            if (id != null) _id = (int)id;
            if (firstName != null) _firstName = firstName;
            if (lastName != null) _lastName = lastName;
            if (phoneNumber != null) _phoneNumber = phoneNumber;
            if (emailAddress != null) _emailAddress = emailAddress;
            if (company != null) _company = company;
            if (icon != null) _icon = icon;
            if (favourite != null) _favourite = (bool)favourite;
            _updated = true;
        }
    }
}
