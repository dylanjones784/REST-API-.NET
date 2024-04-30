using Elfie.Serialization;
using static System.Net.WebRequestMethods;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace sp_project_guide_api.Models
{
    [NotMapped]
    public class Link
    {

        public string Href { get; set; }
        public string Relation { get; set; }
        public string Method { get; set; }
        public string? Type { get; set; }
        public Link() {
        }

        public Link(string href, string relation, string method, int type )
        {
            Href = href;
            Relation = relation;
            Method = method;
            switch (type)
            {
                case 1:
                    Type = "JSON";
                    break;
                case 2:
                    Type = "XML";
                    break;
                case 3:
                    Type = "PLAINTEXT";
                    break;
                case 4:
                    Type = "Custom Security Claim";
                    break;
                default:
                    Type = "None";
                    break;
            }
        }
    }
}
