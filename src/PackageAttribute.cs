using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetPackageChecker {
    public class PackageAttribute {

        public PackageAttribute(string name, string value) {
            this.Name = name;
            this.Value = value;
        }

        public string Name { private set; get; }
        public string Value { private set; get; }
    }
}
