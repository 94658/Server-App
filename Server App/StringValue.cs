using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_App
{
    public class StringValue
    {
        public string _value;
        public StringValue(string s)
        {
            _value = s;
        }

        public string Value { get { return _value; } set { _value = value; } }

    }
}
