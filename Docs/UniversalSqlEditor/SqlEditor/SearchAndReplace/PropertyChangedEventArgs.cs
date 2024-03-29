﻿using System;

namespace SqlEditor.SearchAndReplace
{
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    public class PropertyChangedEventArgs : EventArgs
    {
        global::SqlEditor.SearchAndReplace.Properties properties;
        string key;
        object newValue;
        object oldValue;

        /// <returns>
        /// returns the changed property object
        /// </returns>
        public global::SqlEditor.SearchAndReplace.Properties Properties
        {
            get
            {
                return properties;
            }
        }

        /// <returns>
        /// The key of the changed property
        /// </returns>
        public string Key
        {
            get
            {
                return key;
            }
        }

        /// <returns>
        /// The new value of the property
        /// </returns>
        public object NewValue
        {
            get
            {
                return newValue;
            }
        }

        /// <returns>
        /// The new value of the property
        /// </returns>
        public object OldValue
        {
            get
            {
                return oldValue;
            }
        }

        public PropertyChangedEventArgs(global::SqlEditor.SearchAndReplace.Properties properties, string key, object oldValue, object newValue)
        {
            this.properties = properties;
            this.key = key;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
