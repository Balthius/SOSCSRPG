using Elebris.Library.Units.Values.Handlers;

namespace Elebris_WPF_Rpg.Models
{
    // DTO
    // used for uncomplicated instances where a value needs to be held, but will not be modified

    public class MeasurementObserver
        {

            private readonly ValueHandler _handler;
        private float currentValue;
            private float maxValue;
            private string _observedValue;

            public MeasurementObserver(ValueHandler handler, string observedValue, float missingValue = 0)
            {
                //observedValue; use this to hook up an event
                MaxValue = 1;
                CurrentValue = MaxValue - missingValue;
                _handler = handler;
                _observedValue = observedValue;
                Subscribe(_observedValue);
            }

            public void Subscribe(string parentstat)
            {
                StatValue ParentStat = _handler.RetrieveValue(parentstat);
                //ParentStat.Subscribe(this);
            }

            public void OnStatUpdated()
            {
                float temp= this.CurrentValue;
                MaxValue = _handler.RetrieveValue(_observedValue).TotalValue;
                CurrentValue = temp;
            }

            public float CurrentValue
            {
                get => currentValue;
                set
                {
                    if (currentValue >= MaxValue)
                    {
                        currentValue = MaxValue;
                    }
                    else if (currentValue <= 0)
                    {
                        currentValue = 0;
                    }

                    currentValue = value;
                }
            }
            public float MaxValue
            {
                get
                {
                    return maxValue;
                }

                set
                {
                    float missing = MissingValueRaw;
                    maxValue = value;
                    currentValue = maxValue - missing;
                }
            }


            public float CurrentValuePercent => currentValue / maxValue;
            public float MissingValuePercent => (maxValue - currentValue) / maxValue;
            public float MissingValueRaw => maxValue - currentValue;

        }
}
