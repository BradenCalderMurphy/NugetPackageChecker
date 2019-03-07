using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NugetPackageChecker {
    public class PackageFile {

        public string FileName { private set; get; }

        private List<PackageItem> _packages = new List<PackageItem>();
        public ReadOnlyCollection<PackageItem> Packages {
            get {
                return _packages.AsReadOnly();
            }
        }

        public PackageFile(string fileName) {
            this.FileName = fileName;
            _packages.AddRange(GetPackageItems(fileName));
        }

        private static string DrawStars() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 50; ++i) {
                sb.Append("*");
            }
            return sb.ToString();
        }

        private static List<PackageItem> GetPackageItems(string fileName) {
            XmlDocument doc = new XmlDocument();
            doc.Load(fileName);

            List<PackageItem> lst = new List<PackageItem>();
            XmlNode packageNode = doc.SelectSingleNode("packages");
            if (packageNode == null) {
                return lst;
            }
            XmlNodeList nodes = packageNode.SelectNodes("package");
            if (nodes == null) {
                return lst;
            }

            foreach (XmlNode inner in nodes) {
                if (!inner.Name.ToLower().Equals("package")) continue;

                List<PackageAttribute> attributes = new List<PackageAttribute>();
                string id = "";

                foreach (XmlAttribute attr in inner.Attributes) {
                    if (attr.Name.ToLower().Equals("id")) {
                        id = attr.Value;
                    }
                    else {
                        attributes.Add(new PackageAttribute(attr.Name, attr.Value));
                    }
                }
                lst.Add(new PackageItem(id, attributes));
            }

            return lst;
        }

        public static List<string> Differences(PackageFile file, List<PackageFile> otherFiles) {
            List<string> lst = new List<string>();
            if (file == null || otherFiles == null) {
                lst.Add("CODE ERROR: Invalid Files.");
                return lst;
            }


            foreach (PackageItem item1 in file.Packages) {
                List<string> ignoreAttributes = new List<string>();
                foreach (PackageFile f in otherFiles) {
                    foreach (PackageItem item2 in f.Packages) {
                        List<string> errors = AttributeErrors(f.FileName, item1, item2, ref ignoreAttributes);
                        List<string> errors2 = AttributeErrors(f.FileName, item2, item1, ref ignoreAttributes);

                        if (errors.Count > 0 || errors2.Count > 0) {
                        
                            string strError1 = ListToString(errors);
                            string strError2 = ListToString(errors2);
                            if (!String.IsNullOrWhiteSpace(strError1) &&
                                !String.IsNullOrWhiteSpace(strError2)) {
                                strError2 = "," + strError2;
                            }
                            lst.Add($"{item2.ID} - {strError1}{strError2}");
                        }
                    }
                }
            }

            if(lst.Count > 0) {
                lst.Insert(0, file.FileName);
                lst.Insert(0, DrawStars());
            }
            return lst;
        }

        private static string ListToString(List<string> lst) {
            if (lst == null) return "";
            string result = "";
            lst.ForEach(x => result += x + ",");
            result = result.TrimEnd(',');
            return result;
        }

        private static List<string> AttributeErrors(string otherFile,
                                                    PackageItem pi1,
                                                    PackageItem pi2,
                                                    ref List<string> ignoreAttributes) {
            List<string> lst = new List<string>();

            if (pi1 == null || pi2 == null) return lst;
            if (!String.Equals(pi1.ID, pi2.ID)) return lst;

            bool found = false;
            foreach (PackageAttribute attr1 in pi1.Attributes) {
                foreach (PackageAttribute attr2 in pi2.Attributes) {
                    if (!String.Equals(attr1.Name, attr2.Name)) continue;

                    found = true;
                    if (!String.Equals(attr1.Value, attr2.Value) && !ignoreAttributes.Contains(attr1.Name)) {
                        lst.Add($"Attribute '{attr1.Name}' does not match value '{attr1.Value}' - '{attr2.Value}' ({otherFile})");
                        ignoreAttributes.Add(attr1.Name);
                    }
                    break;
                }

                if (!found && !ignoreAttributes.Contains(attr1.Name)) {
                    lst.Add($"Cannot find attribute '{attr1.Name}' ({otherFile})");
                }
            }
            return lst;
        }
    }
}
