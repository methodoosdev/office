using System.Collections.Generic;

namespace App.Core
{
    public class SelectionItemList
    {
        public SelectionItemList() { }
        public SelectionItemList(int value, string label)
        {
            Value = value;
            Label = label;
        }
        public SelectionItemList(int value, string label, bool disabled) : this(value, label)
        {
            Disabled = disabled; 
        }
        public int Value { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }
    public class SelectionList
    {
        public SelectionList() { }
        public SelectionList(string value, string label)
        {
            Value = value;
            Label = label;
        }
        public SelectionList(string value, string label, bool disabled) : this(value, label)
        {
            Disabled = disabled;
        }
        public string Value { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }
    public class SelectionGroupItemList
    {
        public SelectionGroupItemList() { }
        public SelectionGroupItemList(int value, string label, string group)
        {
            Value = value;
            Label = label;
            GroupItem = group;
        }
        public SelectionGroupItemList(int value, string label, string group, bool disabled) : this(value, label, group)
        {
            Disabled = disabled;
        }
        public int Value { get; set; }
        public string GroupItem { get; set; }
        public string Label { get; set; }
        public bool Disabled { get; set; }
    }
    public class SelectionTraderIdsList
    {
        public int Value { get; set; }
        public string Label { get; set; }
        public List<int> Ids { get; set; }
    }
}