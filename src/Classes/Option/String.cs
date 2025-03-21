using System;

namespace HarryPotter.CustomOption
{
    public class CustomStringOption : CustomOption
    {
        public CustomStringOption(int id, string name, string[] values) : base(id, name,
            CustomOptionType.String,
            0)
        {
            Values = values;
            Format = value => Values[(int)value];
        }

        protected string[] Values { get; set; }

        public int Get()
        {
            return (int)Value;
        }

        public void Increase()
        {
            if (Get() >= Values.Length - 1)
                Set(0);
            else
                Set(Get() + 1);
        }

        public void Decrease()
        {
            if (Get() <= 0)
                Set(Values.Length - 1);
            else
                Set(Get() - 1);
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var str = Setting.Cast<StringOption>();
            str.Value = str.oldValue = Get();
            str.ValueText.text = ToString();
        }
    }
}
