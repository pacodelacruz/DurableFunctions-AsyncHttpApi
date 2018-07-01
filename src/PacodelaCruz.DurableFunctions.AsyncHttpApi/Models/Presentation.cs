using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PacodelaCruz.DurableFunctions.AsyncHttpApi.Models
{
    public class Presentation
    {
        public string Id { get; set; }
        public Speaker Speaker { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Track { get; set; }
    }
}
