using System.Collections.ObjectModel;

namespace Elebris_WPF_Rpg.Models
{
    /// <summary>
    /// https://forum.unity.com/threads/tutorial-character-stats-aka-attributes-system.504095/
    /// will this class bieing serializable ina  DLL allow Unity to also see it in the inspector?
    /// </summary>
    [Serializable]
    public class StatValue
    {
        public float MinimumBound { get; set; } = float.MinValue;
        public float MaximumBound { get; set; } = float.MaxValue;
        public float BaseValue { get; protected set; }
        public float GetAddedValue
        {
            get
            {
                return totalValue - BaseValue;
            }
        }
        protected float totalValue;
        public virtual float TotalValue
        {
            get
            {
                if (isDirty || lastBaseValue != BaseValue)
                {
                    lastBaseValue = BaseValue;
                    totalValue = CalculateFinalValue();
                    isDirty = false;
                }
                if (totalValue > MaximumBound) totalValue = MaximumBound;
                if (totalValue < MinimumBound) totalValue = MinimumBound;
                return totalValue;
            }
        }
        protected bool isDirty = true;
        protected float lastBaseValue = float.MinValue;


        protected readonly List<ValueModifier> valueModifiers;
        public readonly ReadOnlyCollection<ValueModifier> ValueModifiers;
        public StatValue()
        {
            valueModifiers = new List<ValueModifier>();
            ValueModifiers = valueModifiers.AsReadOnly();
        }
        public StatValue(float baseValue) : this()
        {
            this.BaseValue = baseValue;
        }


        protected virtual int CompareModifierOrder(ValueModifier a, ValueModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; // if (a.Order == b.Order)
        }
        public virtual void AddModifier(ValueModifier mod)
        {
            isDirty = true;
            valueModifiers.Add(mod);

            valueModifiers.Sort(CompareModifierOrder);
        }

        public bool RemoveModifier(ValueModifier mod)
        {
            if (valueModifiers.Remove(mod))
            {
                isDirty = true;
                return true;
            }
            return false;
        }


        public virtual bool RemoveAllModifiersFromSource(object source)
        {
            bool didRemove = false;

            for (int i = valueModifiers.Count - 1; i >= 0; i--)
            {
                if (valueModifiers[i].Source == source)
                {
                    isDirty = true;
                    didRemove = true;
                    valueModifiers.RemoveAt(i);
                }
            }
            return didRemove;
        }

        public virtual void RemoveModifiersByDuration()
        {
            //check at the end of each characters round, every tick, depends on the game
            for (int i = valueModifiers.Count - 1; i >= 0; i--)
            {
                if (valueModifiers[i].HasDuration)
                {
                    valueModifiers[i].Duration -= 1;
                    if (valueModifiers[i].Duration <= 0)
                    {
                        valueModifiers.RemoveAt(i);
                    }
                }
            }
        }
        public virtual float CalculateFinalValue()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;

            for (int i = 0; i < valueModifiers.Count; i++)
            {
                ValueModifier mod = valueModifiers[i];

                if (mod.Type == ValueModEnum.Flat)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Type == ValueModEnum.PercentAdd) // When we encounter a "PercentAdd" modifier
                {
                    sumPercentAdd += mod.Value; // Start adding together all modifiers of this type

                    // If we're at the end of the list OR the next modifer isn't of this type
                    if (i + 1 >= valueModifiers.Count || valueModifiers[i + 1].Type != ValueModEnum.PercentAdd)
                    {
                        finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                        sumPercentAdd = 0; // Reset the sum back to 0
                    }
                }
                //last, if there are multiplicative values (which need to be calculated separately from eachother) they are calculated here.
                else if (mod.Type == ValueModEnum.PercentMult)
                {
                    finalValue *= 1 + mod.Value;
                }
            }
            // Rounding gets around dumb float calculation errors (like getting 12.0001f, instead of 12f)
            // 4 significant digits is usually precise enough, but feel free to change this to fit your needs
            return (float)Math.Round(finalValue, 4);
        }


    }
    public class ValueModifier
    {
        public readonly float Value;
        public readonly ValueModEnum Type;
        public readonly int Order;
        public readonly object Source;
        public float Duration;
        public bool HasDuration;

        public ValueModifier(float value, ValueModEnum type, int order, object source, float duration, bool hasDuration = false)
        {
            Value = value;
            Type = type;
            Order = order;
            Source = source; // Assign Source to our new input parameter
            Duration = duration;
            HasDuration = hasDuration;
        }
        // Requires Value and Type. Calls the "Main" 
        //constructor and sets Order and Source to their default values: (int)type and null, respectively.
        public ValueModifier(float value, ValueModEnum type) : this(value, type, (int)type, null, 1) { }

        // Requires Value, Type and Order. Sets Source to its default value: null
        public ValueModifier(float value, ValueModEnum type, int order) : this(value, type, order, null, 1) { }

        // Requires Value, Type and Source. Sets Order to its default value: (int)Type
        public ValueModifier(float value, ValueModEnum type, object source) : this(value, type, (int)type, source, 1) { }
        // Requires Value, Type and Order, Duration. Sets Source to its default value: null
        public ValueModifier(float value, ValueModEnum type, int order, int duration) : this(value, type, order, null, duration) { }

        // Requires Value, Type and Source, Duration. Sets Order to its default value: (int)Type
        public ValueModifier(float value, ValueModEnum type, object source, int duration) : this(value, type, (int)type, source, duration) { }
    }
    public enum ValueModEnum
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300, //these values are calculated one at a time, rather then added and then multiplied as one lump amount
    }
}
