using System;
using System.Collections.Generic;

namespace TrendBlend.pages
{
    public partial class TrendyChat : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // Model classes
        public class ApparelItem
        {
            public int ApparelId { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Size { get; set; }
            public string AccessoryType { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public byte R { get; set; }
            public byte G { get; set; }
            public byte B { get; set; }
            public string ColorName { get; set; }

            public string ColorDescription
            {
                get
                {
                    try
                    {
                        return !string.IsNullOrEmpty(ColorName)
                            ? ColorName
                            : $"RGB({R},{G},{B})";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[TrendyChat] ERROR in ColorDescription: {ex.Message}");
                        return "Color information unavailable";
                    }
                }
            }
        }

        public class OutfitRecommendation
        {
            private static void LogDebug(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[TrendyChat] {DateTime.Now:HH:mm:ss.fff} - OutfitRecommendation: {message}");
            }

            private string _outfitName;
            public string outfitName
            {
                get => _outfitName;
                set
                {
                    _outfitName = value;
                    LogDebug($"Set outfitName: {value}");
                }
            }

            private string _eventType;
            public string eventType
            {
                get => _eventType;
                set
                {
                    _eventType = value;
                    LogDebug($"Set eventType: {value}");
                }
            }

            private string _description;
            public string description
            {
                get => _description;
                set
                {
                    _description = value;
                    LogDebug($"Set description: {value?.Length ?? 0} chars");
                }
            }

            private List<OutfitItem> _items;
            public List<OutfitItem> items
            {
                get => _items;
                set
                {
                    _items = value;
                    LogDebug($"Set items: {value?.Count ?? 0} items");
                }
            }

            private string _stylingTips;
            public string stylingTips
            {
                get => _stylingTips;
                set
                {
                    _stylingTips = value;
                    LogDebug($"Set stylingTips: {value?.Length ?? 0} chars");
                }
            }
        }

        public class OutfitItem
        {
            private static void LogDebug(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[TrendyChat] {DateTime.Now:HH:mm:ss.fff} - OutfitItem: {message}");
            }

            private int _id;
            public int id
            {
                get => _id;
                set
                {
                    _id = value;
                    LogDebug($"Set id: {value}");
                }
            }

            private string _type;
            public string type
            {
                get => _type;
                set
                {
                    _type = value;
                    LogDebug($"Set type: {value}");
                }
            }

            private string _reason;
            public string reason
            {
                get => _reason;
                set
                {
                    _reason = value;
                    LogDebug($"Set reason: {value?.Length ?? 0} chars");
                }
            }
        }

        public class GeminiResponse
        {
            private static void LogDebug(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[TrendyChat] {DateTime.Now:HH:mm:ss.fff} - GeminiResponse: {message}");
            }

            private Candidate[] _candidates;
            public Candidate[] candidates
            {
                get => _candidates;
                set
                {
                    _candidates = value;
                    LogDebug($"Set candidates: {value?.Length ?? 0} items");
                }
            }
        }

        public class Candidate
        {
            private static void LogDebug(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[TrendyChat] {DateTime.Now:HH:mm:ss.fff} - Candidate: {message}");
            }

            private Content _content;
            public Content content
            {
                get => _content;
                set
                {
                    _content = value;
                    LogDebug("Content set");
                }
            }
        }

        public class Content
        {
            private static void LogDebug(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[TrendyChat] {DateTime.Now:HH:mm:ss.fff} - Content: {message}");
            }

            private Part[] _parts;
            public Part[] parts
            {
                get => _parts;
                set
                {
                    _parts = value;
                    LogDebug($"Set parts: {value?.Length ?? 0} items");
                }
            }
        }

        public class Part
        {
            private static void LogDebug(string message)
            {
                System.Diagnostics.Debug.WriteLine($"[TrendyChat] {DateTime.Now:HH:mm:ss.fff} - Part: {message}");
            }

            private string _text;
            public string text
            {
                get => _text;
                set
                {
                    _text = value;
                    LogDebug($"Set text: {value?.Length ?? 0} chars");
                }
            }
        }

    }
}