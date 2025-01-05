using System.Text.Json;
using YarikVor.Json.LazyLinq.JEnitites;

namespace YarikVor.Json.LazyLinq;

public class Class1
{
    public static void Main()
    {
        var t = """
                {
                  "id": 12345,
                  "name": "Jane Doe",
                  "profile": {
                    "personalInfo": {
                      "fullName": "Jane Ann Doe",
                      "birthDate": "1992-07-15",
                      "contacts": [
                        {
                          "type": "email",
                          "value": "jane.doe@example.com"
                        },
                        {
                          "type": "phone",
                          "value": "+1-800-555-0199"
                        }
                      ]
                    },
                    "preferences": {
                      "theme": "dark",
                      "notifications": {
                        "email": true,
                        "sms": false,
                        "push": true
                      },
                      "languages": ["en", "es", "fr"]
                    }
                  },
                  "employment": {
                    "isEmployed": true,
                    "details": {
                      "company": "TechCorp",
                      "position": "Software Engineer",
                      "startDate": "2015-06-01",
                      "endDate": null
                    },
                    "projects": [
                      {
                        "name": "Internal Tools",
                        "role": "Lead Developer",
                        "teamSize": 5,
                        "technologies": ["C#", ".NET", "Azure"],
                        "completed": true
                      },
                      {
                        "name": "Mobile App",
                        "role": "Backend Developer",
                        "teamSize": 8,
                        "technologies": ["Node.js", "MongoDB", "AWS"],
                        "completed": false
                      }
                    ]
                  },
                  "hobbies": ["photography", "traveling", "gaming"],
                  "education": [
                    {
                      "degree": "Bachelor's in Computer Science",
                      "institution": "University of Tech",
                      "yearGraduated": 2014
                    },
                    {
                      "degree": "Master's in Software Engineering",
                      "institution": "Global Tech Institute",
                      "yearGraduated": 2016
                    }
                  ],
                  "meta": {
                    "createdAt": "2024-01-01T12:00:00Z",
                    "updatedAt": "2024-12-30T15:45:00Z",
                    "version": "1.4.2"
                  }
                }
                
                """;
        
        var n = JDocument.GetAndConverted(t, 0);

        /*if (n.Type is JType.Object)
        {
            var o = (JObject)n;
            var a = o.GetProperties()
              .Select(v => v.GetKey().GetValue())
                .ToArray();
        }*/
        
    }
}