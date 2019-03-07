using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetPackageChecker {
    public class PackageItem {
        public string ID { private set; get; }

        private List<PackageAttribute> _attributes = new List<PackageAttribute>();

        public ReadOnlyCollection<PackageAttribute> Attributes {
            get {
                return _attributes.AsReadOnly();
            }
        }

        public PackageItem(string id, List<PackageAttribute> attributes) {
            this.ID = id;
            if (attributes != null) {
                this._attributes.AddRange(attributes);
            }
        }

    }
}
