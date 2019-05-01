
using System.CodeDom.Compiler;

namespace QuickBooks
{

    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Collections.Generic;


    #region PropertyValueState class
    public class PropertyValueState
    {

        public string PropertyName { get; set; }
        public object OriginalValue { get; set; }
        public object CurrentValue { get; set; }
        public ObjectState State { get; set; }
    }
    #endregion

    #region ObjectStateChangingEventArgs class
    public class ObjectStateChangingEventArgs : EventArgs
    {

        public ObjectState NewState { get; set; }
    }
    #endregion

    #region ObjectList class
    public class ObjectList : List<object>
    {
    }
    #endregion

    #region ObjectState enum
    public enum ObjectState
    {

        Unchanged,

        Added,

        Modified,

        Deleted,
    }
    #endregion

    #region NotifyTrackableCollectionChangedEventHandler class
    public delegate void NotifyTrackableCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e, string propertyName);
    #endregion

    #region Tracking changes class
    public class ObjectChangeTracker : INotifyPropertyChanged
    {

        #region  Fields
        private bool isDeserializingField;
        private ObjectState originalobjectStateField = ObjectState.Added;
        private bool isInitilizedField = false;
        private readonly ObservableCollection<PropertyValueState> changeSetsField = new ObservableCollection<PropertyValueState>();
        private Delegate collectionChangedDelegateField = null;

        private bool objectTrackingEnabledField;
        private readonly object trackedObjectField;

        private PropertyValueStatesDictionary propertyValueStatesFields;
        //private ValueStatesDictionary valueStatesField;

        private ObjectsAddedToCollectionProperties objectsAddedToCollectionsField = new ObjectsAddedToCollectionProperties();
        private ObjectsRemovedFromCollectionProperties objectsRemovedFromCollectionsField = new ObjectsRemovedFromCollectionProperties();
        private ObjectsOriginalFromCollectionProperties objectsOriginalFromCollectionsField = new ObjectsOriginalFromCollectionProperties();
        #endregion

        public ObjectChangeTracker(object trackedObject)
        {
            trackedObjectField = trackedObject;
        }

        #region Events

        public event EventHandler<ObjectStateChangingEventArgs> ObjectStateChanging;

        #endregion

        protected virtual void OnObjectStateChanging(ObjectState newState)
        {
            if (ObjectStateChanging != null)
            {
                ObjectStateChanging(this, new ObjectStateChangingEventArgs() { NewState = newState });
            }
        }

        /// <summary>
        /// indicate current state
        /// </summary>
        private ObjectState stateField;

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public ObjectState State
        {
            get
            {
                return stateField;
            }
        }

        /// <summary>
        /// Updates the state of the change.
        /// </summary>
        private void UpdateChangeState()
        {
            var resultState = ObjectState.Added;
            changeSetsField.Clear();
            if (originalobjectStateField == ObjectState.Added)
            {
                if (stateField != ObjectState.Added)
                {
                    stateField = ObjectState.Added;
                    OnPropertyChanged("State");
                    OnObjectStateChanging(stateField);
                }
                return;
            }
            foreach (var propertyValuestate in PropertyValueStates)
            {
                if (propertyValuestate.Value.State == ObjectState.Modified)
                {
                    changeSetsField.Add(propertyValuestate.Value);
                    resultState = ObjectState.Modified;
                }
            }

            if (ObjectsRemovedFromCollectionProperties.Count > 0)
            {
                foreach (var objectsRemovedFromCollectionProperty in ObjectsRemovedFromCollectionProperties)
                {
                    foreach (var objectsRemoved in objectsRemovedFromCollectionProperty.Value)
                    {
                        changeSetsField.Add(new PropertyValueState() { PropertyName = objectsRemovedFromCollectionProperty.Key, State = ObjectState.Deleted, CurrentValue = objectsRemoved.ToString() });
                    }
                }
                resultState = ObjectState.Modified;
            }

            if (ObjectsAddedToCollectionProperties.Count > 0)
            {
                foreach (var objectsAddedFromCollectionProperty in ObjectsAddedToCollectionProperties)
                {
                    foreach (var objectsAdded in objectsAddedFromCollectionProperty.Value)
                    {
                        changeSetsField.Add(new PropertyValueState() { PropertyName = objectsAddedFromCollectionProperty.Key, State = ObjectState.Added, CurrentValue = objectsAdded.ToString() });
                    }
                }
                resultState = ObjectState.Modified;
            }

            if (resultState == ObjectState.Modified)
            {
                if (stateField != ObjectState.Modified)
                {
                    stateField = ObjectState.Modified;
                    OnPropertyChanged("State");
                    OnObjectStateChanging(stateField);
                }
                return;
            }
            if (stateField != originalobjectStateField)
            {
                stateField = originalobjectStateField;
                OnPropertyChanged("State");
                OnObjectStateChanging(stateField);
            }
            return;
        }

        public ObservableCollection<PropertyValueState> ChangeSets
        {
            get
            {
                return changeSetsField;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [change tracking enabled].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [change tracking enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool ObjectTrackingEnabled
        {
            get { return objectTrackingEnabledField; }
        }

        /// <summary>
        /// Starts record changes.
        /// </summary>
        public void StartTracking()
        {
            objectTrackingEnabledField = true;
            ResetTracking();
        }

        /// <summary>
        /// Starts record changes.
        /// </summary>
        public void StartTracking(bool keepLastRecords)
        {
            objectTrackingEnabledField = true;
            if (!keepLastRecords)
                StartTracking();
        }

        /// <summary>
        /// Starts record changes.
        /// </summary>
        public void StopTracking()
        {
            objectTrackingEnabledField = false;
        }

        // Resets the ObjectChangeTracker to the Unchanged state and
        // clears the original values as well as the record of changes
        // to collection properties
        public void ResetTracking()
        {
            if (objectTrackingEnabledField)
            {
                originalobjectStateField = ObjectState.Unchanged;
                ResetOriginalValue();
                ObjectsAddedToCollectionProperties.Clear();
                ObjectsRemovedFromCollectionProperties.Clear();
                ObjectsOriginalFromCollectionProperties.Clear();
                InitOriginalValue();
                UpdateChangeState();
            }
        }
        /// <summary>
        /// Inits the original value.
        /// </summary>
        private void InitOriginalValue()
        {
            var type = trackedObjectField.GetType();
            var propertyies = type.GetProperties();

            if (!isInitilizedField)
            {
                foreach (var propertyInfo in propertyies)
                {
                    if (!propertyInfo.Name.Equals("ChangeTracker") && !propertyInfo.Name.Equals("Item"))
                    {
                        var o = propertyInfo.GetValue(trackedObjectField, null);
                        if (propertyInfo.PropertyType.Name.Contains("TrackableCollection"))
                        {
                            var eventInfo = propertyInfo.PropertyType.GetEvent("TrackableCollectionChanged");
                            if (eventInfo != null)
                            {
                                var tDelegate = eventInfo.EventHandlerType;
                                if (tDelegate != null)
                                {
                                    if (collectionChangedDelegateField == null)
                                        collectionChangedDelegateField = Delegate.CreateDelegate(tDelegate, this, "TrackableCollectionChanged");

                                    // Get the "add" accessor of the event and invoke it late bound, passing in the delegate instance. This is equivalent
                                    // to using the += operator in C#. The instance on which the "add" accessor is invoked.
                                    var addHandler = eventInfo.GetAddMethod();
                                    Object[] addHandlerArgs = { collectionChangedDelegateField };
                                    addHandler.Invoke(o, addHandlerArgs);
                                }
                            }

                            var collection = o as IEnumerable;
                            if (collection != null)
                            {
                                foreach (var item in collection)
                                {
                                    RecordOriginalToCollectionProperties(propertyInfo.Name, item);
                                }
                            }
                        }
                        else
                        {
                            RecordCurrentValue(propertyInfo.Name, o);
                        }
                    }
                }
                isInitilizedField = true;
            }
        }

        /// <summary>
        /// Resets the original value.
        /// </summary>
        private void ResetOriginalValue()
        {
            PropertyValueStates.Clear();
            //ValueStates.Clear();

            if (isInitilizedField)
            {
                var type = trackedObjectField.GetType();
                var propertyies = type.GetProperties();
                foreach (var propertyInfo in propertyies)
                {
                    if (!propertyInfo.Name.Equals("ChangeTracker") && !propertyInfo.Name.Equals("Item"))
                    {
                        var o = propertyInfo.GetValue(trackedObjectField, null);
                        if (propertyInfo.PropertyType.Name.Contains("TrackableCollection"))
                        {
                            var eventInfo = propertyInfo.PropertyType.GetEvent("TrackableCollectionChanged");
                            if (eventInfo != null)
                            {
                                var tDelegate = eventInfo.EventHandlerType;
                                if (tDelegate != null)
                                {
                                    if (collectionChangedDelegateField != null)
                                    {
                                        // Get the "remove" accessor of the event and invoke it latebound, passing in the delegate instance. This is equivalent
                                        // to using the += operator in C#. The instance on which the "add" accessor is invoked.
                                        var removeHandler = eventInfo.GetRemoveMethod();
                                        Object[] addHandlerArgs = { collectionChangedDelegateField };
                                        removeHandler.Invoke(o, addHandlerArgs);
                                    }
                                }
                            }
                        }
                    }
                }
                isInitilizedField = false;
            }
        }

        /// <summary>
        /// Trackables the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyTrackableCollectionChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="propertyName">Name of the property.</param>
        private void TrackableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e, string propertyName)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var newItem in e.NewItems)
                    {
                        //todo:implémenter la récursivité sur l'ajout des élements dans la collection
                        //var project = newItem as IObjectWithChangeTracker;
                        //if (project != null)
                        //{
                        //    if (this.ChangeTrackingEnabled)
                        //    {
                        //        project.ChangeTracker.Start();
                        //    }
                        //}
                        RecordAdditionToCollectionProperties(propertyName, newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var odlItem in e.OldItems)
                    {
                        RecordRemovalFromCollectionProperties(propertyName, odlItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        // Cas d'un Clear sur la collection.
                        // Vide le cache des modifications pour la collection.
                        if (ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName))
                        {
                            ObjectsRemovedFromCollectionProperties.Remove(propertyName);
                        }

                        if (ObjectsAddedToCollectionProperties.ContainsKey(propertyName))
                        {
                            ObjectsAddedToCollectionProperties.Remove(propertyName);
                        }

                        // Tranfère les données de départ dans les élements supprimés.
                        if (ObjectsOriginalFromCollectionProperties.Count > 0)
                        {
                            foreach (var item in ObjectsOriginalFromCollectionProperties[propertyName])
                            {
                                RecordRemovalFromCollectionProperties(propertyName, item);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        // Comment gérer le cas d'un changement d'instance dans la liste ?
                    }
                    break;
            }
            UpdateChangeState();
        }

        #region public properties
        // Returns the removed objects to collection valued properties that were changed.
        [DataMember]
        public ObjectsRemovedFromCollectionProperties ObjectsRemovedFromCollectionProperties
        {
            get { return objectsRemovedFromCollectionsField ?? (objectsRemovedFromCollectionsField = new ObjectsRemovedFromCollectionProperties()); }
        }

        // Returns the original values for properties that were changed.
        [DataMember]
        public PropertyValueStatesDictionary PropertyValueStates
        {
            get { return propertyValueStatesFields ?? (propertyValueStatesFields = new PropertyValueStatesDictionary()); }
        }

        // Returns the added objects to collection valued properties that were changed.
        [DataMember]
        public ObjectsAddedToCollectionProperties ObjectsAddedToCollectionProperties
        {
            get { return objectsAddedToCollectionsField ?? (objectsAddedToCollectionsField = new ObjectsAddedToCollectionProperties()); }
        }

        // Returns the added objects to collection valued properties that were changed.
        [DataMember]
        public ObjectsOriginalFromCollectionProperties ObjectsOriginalFromCollectionProperties
        {
            get { return objectsOriginalFromCollectionsField ?? (objectsOriginalFromCollectionsField = new ObjectsOriginalFromCollectionProperties()); }
        }

        #region MethodsForChangeTrackingOnClient

        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext conText)
        {
            isDeserializingField = true;
        }

        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext conText)
        {
            isDeserializingField = false;
        }
        #endregion

        /// <summary>
        /// Captures the original value for a property that is changing.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        public void RecordCurrentValue(string propertyName, object value)
        {
            if (objectTrackingEnabledField)
            {
                if (!PropertyValueStates.ContainsKey(propertyName))
                {
                    PropertyValueStates[propertyName] = new PropertyValueState() { PropertyName = propertyName, OriginalValue = value, CurrentValue = value, State = ObjectState.Unchanged };
                }
                // Compare original value new 
                else
                {
                    PropertyValueStates[propertyName].CurrentValue = value;
                    var originalValue = PropertyValueStates[propertyName].OriginalValue;
                    if (originalValue != null)
                    {
                        PropertyValueStates[propertyName].State = originalValue.Equals(value) ? ObjectState.Unchanged : ObjectState.Modified;
                    }
                    else
                    {
                        if (value == null)
                        {
                            PropertyValueStates[propertyName].State = ObjectState.Unchanged;
                        }
                        else
                        {
                            PropertyValueStates[propertyName].State = string.IsNullOrEmpty(value.ToString()) ? ObjectState.Unchanged : ObjectState.Modified;
                        }
                    }
                }
                UpdateChangeState();
            }
        }

        /// <summary>
        /// Records original items value of collection to collection valued properties on SelfTracking Entities.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        private void RecordOriginalToCollectionProperties(string propertyName, object value)
        {
            if (objectTrackingEnabledField)
            {
                if (!ObjectsOriginalFromCollectionProperties.ContainsKey(propertyName))
                {
                    ObjectsOriginalFromCollectionProperties[propertyName] = new ObjectList();
                    ObjectsOriginalFromCollectionProperties[propertyName].Add(value);
                }
                else
                {
                    ObjectsOriginalFromCollectionProperties[propertyName].Add(value);
                }
            }
        }


        /// <summary>
        /// Records an addition to collection valued properties on SelfTracking Entities.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        private void RecordAdditionToCollectionProperties(string propertyName, object value)
        {
            if (objectTrackingEnabledField)
            {
                // Add the entity back after deleting it, we should do nothing here then
                if (ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName)
                    && ObjectsRemovedFromCollectionProperties[propertyName].Contains(value))
                {
                    ObjectsRemovedFromCollectionProperties[propertyName].Remove(value);
                    if (ObjectsRemovedFromCollectionProperties[propertyName].Count == 0)
                    {
                        ObjectsRemovedFromCollectionProperties.Remove(propertyName);
                    }
                    return;
                }

                if (!ObjectsAddedToCollectionProperties.ContainsKey(propertyName))
                {
                    ObjectsAddedToCollectionProperties[propertyName] = new ObjectList();
                    ObjectsAddedToCollectionProperties[propertyName].Add(value);
                }
                else
                {
                    ObjectsAddedToCollectionProperties[propertyName].Add(value);
                }
            }
        }

        /// <summary>
        /// Records a removal to collection valued properties on SelfTracking Entities.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The object value.</param>
        internal void RecordRemovalFromCollectionProperties(string propertyName, object value)
        {
            if (objectTrackingEnabledField)
            {
                // Delete the entity back after adding it, we should do nothing here then
                if (ObjectsAddedToCollectionProperties.ContainsKey(propertyName)
                    && ObjectsAddedToCollectionProperties[propertyName].Contains(value))
                {
                    ObjectsAddedToCollectionProperties[propertyName].Remove(value);
                    if (ObjectsAddedToCollectionProperties[propertyName].Count == 0)
                    {
                        ObjectsAddedToCollectionProperties.Remove(propertyName);
                    }
                    return;
                }

                if (!ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName))
                {
                    ObjectsRemovedFromCollectionProperties[propertyName] = new ObjectList();
                    ObjectsRemovedFromCollectionProperties[propertyName].Add(value);
                }
                else
                {
                    if (!ObjectsRemovedFromCollectionProperties[propertyName].Contains(value))
                    {
                        ObjectsRemovedFromCollectionProperties[propertyName].Add(value);
                    }
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="info">The info.</param>
        public virtual void OnPropertyChanged(string info)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
    #endregion

    #region TrackableCollection class
    public class TrackableCollection<T> : ObservableCollection<T>
    {

        /// <summary>
        /// Name of property
        /// </summary>
        private string propertyNameField;

        /// <summary>
        /// Occurs when [trackable collection changed].
        /// </summary>
        public virtual event NotifyTrackableCollectionChangedEventHandler TrackableCollectionChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackableCollection&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public TrackableCollection(string propertyName)
        {
            propertyNameField = propertyName;
            base.CollectionChanged += TrackableCollection_CollectionChanged;
        }

        /// <summary>
        /// Handles the CollectionChanged event of the TrackableCollection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        void TrackableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (TrackableCollectionChanged != null)
            {
                TrackableCollectionChanged(sender, e, propertyNameField);
            }
        }


    }
    #endregion

    #region PropertyValueStatesDictionary
    public class PropertyValueStatesDictionary : Dictionary<string, PropertyValueState>
    {
    }
    #endregion

    #region ObjectsRemovedFromCollectionProperties
    public class ObjectsRemovedFromCollectionProperties : Dictionary<string, ObjectList>
    {
    }
    #endregion

    #region ObjectsAddedToCollectionProperties
    public class ObjectsAddedToCollectionProperties : Dictionary<string, ObjectList>
    {
    }
    #endregion

    #region ObjectsOriginalFromCollectionProperties
    public class ObjectsOriginalFromCollectionProperties : Dictionary<string, ObjectList>
    {
    }
    #endregion

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    [DataContract(Name = "SalesReceiptRet")]
    public partial class SalesReceiptRet : INotifyPropertyChanged
    {

        private string txnIDField;

        private DateTime timeCreatedField;

        private DateTime timeModifiedField;

        private string associateField;

        private string cashierField;

        private string commentsField;

        private string customerListIDField;

        private decimal discountField;

        private decimal discountPercentField;

        private string historyDocStatusField;

        private string itemsCountField;

        private int priceLevelNumberField;

        private string promoCodeField;

        private string quickBooksFlagField;

        private string salesOrderTxnIDField;

        private string salesReceiptNumberField;

        private string salesReceiptTypeField;

        private DateTime shipDateField;

        private string storeExchangeStatusField;

        private string storeNumberField;

        private decimal subtotalField;

        private decimal taxAmountField;

        private string taxCategoryField;

        private decimal taxPercentageField;

        private string tenderTypeField;

        private string tipReceiverField;

        private decimal totalField;

        private string trackingNumberField;

        private DateTime txnDateField;

        private string txnStateField;

        private string workstationField;

        private SalesReceiptRetBillingInformation billingInformationField;

        private SalesReceiptRetShippingInformation shippingInformationField;

        private SalesReceiptRetSalesReceiptItemRet salesReceiptItemRetField;

        private SalesReceiptRetTenderAccountRet tenderAccountRetField;

        private SalesReceiptRetTenderCashRet tenderCashRetField;

        private SalesReceiptRetTenderCheckRet tenderCheckRetField;

        private SalesReceiptRetTenderCreditCardRet tenderCreditCardRetField;

        private SalesReceiptRetTenderDebitCardRet tenderDebitCardRetField;

        private SalesReceiptRetTenderDepositRet tenderDepositRetField;

        private SalesReceiptRetTenderGiftRet tenderGiftRetField;

        private SalesReceiptRetTenderGiftCardRet tenderGiftCardRetField;

        private SalesReceiptRetDataExtRet dataExtRetField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        public SalesReceiptRet()
        {
            dataExtRetField = new SalesReceiptRetDataExtRet();
            tenderGiftCardRetField = new SalesReceiptRetTenderGiftCardRet();
            tenderGiftRetField = new SalesReceiptRetTenderGiftRet();
            tenderDepositRetField = new SalesReceiptRetTenderDepositRet();
            tenderDebitCardRetField = new SalesReceiptRetTenderDebitCardRet();
            tenderCreditCardRetField = new SalesReceiptRetTenderCreditCardRet();
            tenderCheckRetField = new SalesReceiptRetTenderCheckRet();
            tenderCashRetField = new SalesReceiptRetTenderCashRet();
            tenderAccountRetField = new SalesReceiptRetTenderAccountRet();
            salesReceiptItemRetField = new SalesReceiptRetSalesReceiptItemRet();
            shippingInformationField = new SalesReceiptRetShippingInformation();
            billingInformationField = new SalesReceiptRetBillingInformation();
        }

        [XmlElement(Order = 0)]
        [DataMember()]
        public string TxnID
        {
            get
            {
                return txnIDField;
            }
            set
            {
                if (((txnIDField == null)
                            || (txnIDField.Equals(value) != true)))
                {
                    txnIDField = value;
                    OnPropertyChanged("TxnID", value);
                }
            }
        }

        [XmlElement(DataType = "date", Order = 1)]
        [DataMember()]
        public DateTime TimeCreated
        {
            get
            {
                return timeCreatedField;
            }
            set
            {
                if ((timeCreatedField.Equals(value) != true))
                {
                    timeCreatedField = value;
                    OnPropertyChanged("TimeCreated", value);
                }
            }
        }

        [XmlElement(DataType = "date", Order = 2)]
        [DataMember()]
        public DateTime TimeModified
        {
            get
            {
                return timeModifiedField;
            }
            set
            {
                if ((timeModifiedField.Equals(value) != true))
                {
                    timeModifiedField = value;
                    OnPropertyChanged("TimeModified", value);
                }
            }
        }

        [XmlElement(Order = 3)]
        [DataMember()]
        public string Associate
        {
            get
            {
                return associateField;
            }
            set
            {
                if (((associateField == null)
                            || (associateField.Equals(value) != true)))
                {
                    associateField = value;
                    OnPropertyChanged("Associate", value);
                }
            }
        }

        [XmlElement(Order = 4)]
        [DataMember()]
        public string Cashier
        {
            get
            {
                return cashierField;
            }
            set
            {
                if (((cashierField == null)
                            || (cashierField.Equals(value) != true)))
                {
                    cashierField = value;
                    OnPropertyChanged("Cashier", value);
                }
            }
        }

        [XmlElement(Order = 5)]
        [DataMember()]
        public string Comments
        {
            get
            {
                return commentsField;
            }
            set
            {
                if (((commentsField == null)
                            || (commentsField.Equals(value) != true)))
                {
                    commentsField = value;
                    OnPropertyChanged("Comments", value);
                }
            }
        }

        [XmlElement(Order = 6)]
        [DataMember()]
        public string CustomerListID
        {
            get
            {
                return customerListIDField;
            }
            set
            {
                if (((customerListIDField == null)
                            || (customerListIDField.Equals(value) != true)))
                {
                    customerListIDField = value;
                    OnPropertyChanged("CustomerListID", value);
                }
            }
        }

        [XmlElement(Order = 7)]
        [DataMember()]
        public decimal Discount
        {
            get
            {
                return discountField;
            }
            set
            {
                if ((discountField.Equals(value) != true))
                {
                    discountField = value;
                    OnPropertyChanged("Discount", value);
                }
            }
        }

        [XmlElement(Order = 8)]
        [DataMember()]
        public decimal DiscountPercent
        {
            get
            {
                return discountPercentField;
            }
            set
            {
                if ((discountPercentField.Equals(value) != true))
                {
                    discountPercentField = value;
                    OnPropertyChanged("DiscountPercent", value);
                }
            }
        }

        [XmlElement(Order = 9)]
        [DataMember()]
        public string HistoryDocStatus
        {
            get
            {
                return historyDocStatusField;
            }
            set
            {
                if (((historyDocStatusField == null)
                            || (historyDocStatusField.Equals(value) != true)))
                {
                    historyDocStatusField = value;
                    OnPropertyChanged("HistoryDocStatus", value);
                }
            }
        }

        [XmlElement(Order = 10)]
        [DataMember()]
        public string ItemsCount
        {
            get
            {
                return itemsCountField;
            }
            set
            {
                if (((itemsCountField == null)
                            || (itemsCountField.Equals(value) != true)))
                {
                    itemsCountField = value;
                    OnPropertyChanged("ItemsCount", value);
                }
            }
        }

        [XmlElement(Order = 11)]
        [DataMember()]
        public int PriceLevelNumber
        {
            get
            {
                return priceLevelNumberField;
            }
            set
            {
                if ((priceLevelNumberField.Equals(value) != true))
                {
                    priceLevelNumberField = value;
                    OnPropertyChanged("PriceLevelNumber", value);
                }
            }
        }

        [XmlElement(Order = 12)]
        [DataMember()]
        public string PromoCode
        {
            get
            {
                return promoCodeField;
            }
            set
            {
                if (((promoCodeField == null)
                            || (promoCodeField.Equals(value) != true)))
                {
                    promoCodeField = value;
                    OnPropertyChanged("PromoCode", value);
                }
            }
        }

        [XmlElement(Order = 13)]
        [DataMember()]
        public string QuickBooksFlag
        {
            get
            {
                return quickBooksFlagField;
            }
            set
            {
                if (((quickBooksFlagField == null)
                            || (quickBooksFlagField.Equals(value) != true)))
                {
                    quickBooksFlagField = value;
                    OnPropertyChanged("QuickBooksFlag", value);
                }
            }
        }

        [XmlElement(Order = 14)]
        [DataMember()]
        public string SalesOrderTxnID
        {
            get
            {
                return salesOrderTxnIDField;
            }
            set
            {
                if (((salesOrderTxnIDField == null)
                            || (salesOrderTxnIDField.Equals(value) != true)))
                {
                    salesOrderTxnIDField = value;
                    OnPropertyChanged("SalesOrderTxnID", value);
                }
            }
        }

        [XmlElement(Order = 15)]
        [DataMember()]
        public string SalesReceiptNumber
        {
            get
            {
                return salesReceiptNumberField;
            }
            set
            {
                if (((salesReceiptNumberField == null)
                            || (salesReceiptNumberField.Equals(value) != true)))
                {
                    salesReceiptNumberField = value;
                    OnPropertyChanged("SalesReceiptNumber", value);
                }
            }
        }

        [XmlElement(Order = 16)]
        [DataMember()]
        public string SalesReceiptType
        {
            get
            {
                return salesReceiptTypeField;
            }
            set
            {
                if (((salesReceiptTypeField == null)
                            || (salesReceiptTypeField.Equals(value) != true)))
                {
                    salesReceiptTypeField = value;
                    OnPropertyChanged("SalesReceiptType", value);
                }
            }
        }

        [XmlElement(DataType = "date", Order = 17)]
        [DataMember()]
        public DateTime ShipDate
        {
            get
            {
                return shipDateField;
            }
            set
            {
                if ((shipDateField.Equals(value) != true))
                {
                    shipDateField = value;
                    OnPropertyChanged("ShipDate", value);
                }
            }
        }

        [XmlElement(Order = 18)]
        [DataMember()]
        public string StoreExchangeStatus
        {
            get
            {
                return storeExchangeStatusField;
            }
            set
            {
                if (((storeExchangeStatusField == null)
                            || (storeExchangeStatusField.Equals(value) != true)))
                {
                    storeExchangeStatusField = value;
                    OnPropertyChanged("StoreExchangeStatus", value);
                }
            }
        }

        [XmlElement(Order = 19)]
        [DataMember()]
        public string StoreNumber
        {
            get
            {
                return storeNumberField;
            }
            set
            {
                if (((storeNumberField == null)
                            || (storeNumberField.Equals(value) != true)))
                {
                    storeNumberField = value;
                    OnPropertyChanged("StoreNumber", value);
                }
            }
        }

        [XmlElement(Order = 20)]
        [DataMember()]
        public decimal Subtotal
        {
            get
            {
                return subtotalField;
            }
            set
            {
                if ((subtotalField.Equals(value) != true))
                {
                    subtotalField = value;
                    OnPropertyChanged("Subtotal", value);
                }
            }
        }

        [XmlElement(Order = 21)]
        [DataMember()]
        public decimal TaxAmount
        {
            get
            {
                return taxAmountField;
            }
            set
            {
                if ((taxAmountField.Equals(value) != true))
                {
                    taxAmountField = value;
                    OnPropertyChanged("TaxAmount", value);
                }
            }
        }

        [XmlElement(Order = 22)]
        [DataMember()]
        public string TaxCategory
        {
            get
            {
                return taxCategoryField;
            }
            set
            {
                if (((taxCategoryField == null)
                            || (taxCategoryField.Equals(value) != true)))
                {
                    taxCategoryField = value;
                    OnPropertyChanged("TaxCategory", value);
                }
            }
        }

        [XmlElement(Order = 23)]
        [DataMember()]
        public decimal TaxPercentage
        {
            get
            {
                return taxPercentageField;
            }
            set
            {
                if ((taxPercentageField.Equals(value) != true))
                {
                    taxPercentageField = value;
                    OnPropertyChanged("TaxPercentage", value);
                }
            }
        }

        [XmlElement(Order = 24)]
        [DataMember()]
        public string TenderType
        {
            get
            {
                return tenderTypeField;
            }
            set
            {
                if (((tenderTypeField == null)
                            || (tenderTypeField.Equals(value) != true)))
                {
                    tenderTypeField = value;
                    OnPropertyChanged("TenderType", value);
                }
            }
        }

        [XmlElement(Order = 25)]
        [DataMember()]
        public string TipReceiver
        {
            get
            {
                return tipReceiverField;
            }
            set
            {
                if (((tipReceiverField == null)
                            || (tipReceiverField.Equals(value) != true)))
                {
                    tipReceiverField = value;
                    OnPropertyChanged("TipReceiver", value);
                }
            }
        }

        [XmlElement(Order = 26)]
        [DataMember()]
        public decimal Total
        {
            get
            {
                return totalField;
            }
            set
            {
                if ((totalField.Equals(value) != true))
                {
                    totalField = value;
                    OnPropertyChanged("Total", value);
                }
            }
        }

        [XmlElement(Order = 27)]
        [DataMember()]
        public string TrackingNumber
        {
            get
            {
                return trackingNumberField;
            }
            set
            {
                if (((trackingNumberField == null)
                            || (trackingNumberField.Equals(value) != true)))
                {
                    trackingNumberField = value;
                    OnPropertyChanged("TrackingNumber", value);
                }
            }
        }

        [XmlElement(DataType = "date", Order = 28)]
        [DataMember()]
        public DateTime TxnDate
        {
            get
            {
                return txnDateField;
            }
            set
            {
                if ((txnDateField.Equals(value) != true))
                {
                    txnDateField = value;
                    OnPropertyChanged("TxnDate", value);
                }
            }
        }

        [XmlElement(Order = 29)]
        [DataMember()]
        public string TxnState
        {
            get
            {
                return txnStateField;
            }
            set
            {
                if (((txnStateField == null)
                            || (txnStateField.Equals(value) != true)))
                {
                    txnStateField = value;
                    OnPropertyChanged("TxnState", value);
                }
            }
        }

        [XmlElement(Order = 30)]
        [DataMember()]
        public string Workstation
        {
            get
            {
                return workstationField;
            }
            set
            {
                if (((workstationField == null)
                            || (workstationField.Equals(value) != true)))
                {
                    workstationField = value;
                    OnPropertyChanged("Workstation", value);
                }
            }
        }

        [XmlElement(Order = 31)]
        [DataMember()]
        public SalesReceiptRetBillingInformation BillingInformation
        {
            get
            {
                return billingInformationField;
            }
            set
            {
                if (((billingInformationField == null)
                            || (billingInformationField.Equals(value) != true)))
                {
                    billingInformationField = value;
                    OnPropertyChanged("BillingInformation", value);
                }
            }
        }

        [XmlElement(Order = 32)]
        [DataMember()]
        public SalesReceiptRetShippingInformation ShippingInformation
        {
            get
            {
                return shippingInformationField;
            }
            set
            {
                if (((shippingInformationField == null)
                            || (shippingInformationField.Equals(value) != true)))
                {
                    shippingInformationField = value;
                    OnPropertyChanged("ShippingInformation", value);
                }
            }
        }

        public TrackableCollection<SalesReceiptRetSalesReceiptItemRet> SalesReceiptItems
        {
        get; set;
        }


        [XmlElement(Order = 33)]
        [DataMember()]
        public SalesReceiptRetSalesReceiptItemRet SalesReceiptItemRet
        {
            get
            {
                return salesReceiptItemRetField;
            }
            set
            {
                if (((salesReceiptItemRetField == null)
                            || (salesReceiptItemRetField.Equals(value) != true)))
                {
                    salesReceiptItemRetField = value;
                    OnPropertyChanged("SalesReceiptItemRet", value);
                }
            }
        }

        [XmlElement(Order = 34)]
        [DataMember()]
        public SalesReceiptRetTenderAccountRet TenderAccountRet
        {
            get
            {
                return tenderAccountRetField;
            }
            set
            {
                if (((tenderAccountRetField == null)
                            || (tenderAccountRetField.Equals(value) != true)))
                {
                    tenderAccountRetField = value;
                    OnPropertyChanged("TenderAccountRet", value);
                }
            }
        }

        [XmlElement(Order = 35)]
        [DataMember()]
        public SalesReceiptRetTenderCashRet TenderCashRet
        {
            get
            {
                return tenderCashRetField;
            }
            set
            {
                if (((tenderCashRetField == null)
                            || (tenderCashRetField.Equals(value) != true)))
                {
                    tenderCashRetField = value;
                    OnPropertyChanged("TenderCashRet", value);
                }
            }
        }

        [XmlElement(Order = 36)]
        [DataMember()]
        public SalesReceiptRetTenderCheckRet TenderCheckRet
        {
            get
            {
                return tenderCheckRetField;
            }
            set
            {
                if (((tenderCheckRetField == null)
                            || (tenderCheckRetField.Equals(value) != true)))
                {
                    tenderCheckRetField = value;
                    OnPropertyChanged("TenderCheckRet", value);
                }
            }
        }

        [XmlElement(Order = 37)]
        [DataMember()]
        public SalesReceiptRetTenderCreditCardRet TenderCreditCardRet
        {
            get
            {
                return tenderCreditCardRetField;
            }
            set
            {
                if (((tenderCreditCardRetField == null)
                            || (tenderCreditCardRetField.Equals(value) != true)))
                {
                    tenderCreditCardRetField = value;
                    OnPropertyChanged("TenderCreditCardRet", value);
                }
            }
        }

        [XmlElement(Order = 38)]
        [DataMember()]
        public SalesReceiptRetTenderDebitCardRet TenderDebitCardRet
        {
            get
            {
                return tenderDebitCardRetField;
            }
            set
            {
                if (((tenderDebitCardRetField == null)
                            || (tenderDebitCardRetField.Equals(value) != true)))
                {
                    tenderDebitCardRetField = value;
                    OnPropertyChanged("TenderDebitCardRet", value);
                }
            }
        }

        [XmlElement(Order = 39)]
        [DataMember()]
        public SalesReceiptRetTenderDepositRet TenderDepositRet
        {
            get
            {
                return tenderDepositRetField;
            }
            set
            {
                if (((tenderDepositRetField == null)
                            || (tenderDepositRetField.Equals(value) != true)))
                {
                    tenderDepositRetField = value;
                    OnPropertyChanged("TenderDepositRet", value);
                }
            }
        }

        [XmlElement(Order = 40)]
        [DataMember()]
        public SalesReceiptRetTenderGiftRet TenderGiftRet
        {
            get
            {
                return tenderGiftRetField;
            }
            set
            {
                if (((tenderGiftRetField == null)
                            || (tenderGiftRetField.Equals(value) != true)))
                {
                    tenderGiftRetField = value;
                    OnPropertyChanged("TenderGiftRet", value);
                }
            }
        }

        [XmlElement(Order = 41)]
        [DataMember()]
        public SalesReceiptRetTenderGiftCardRet TenderGiftCardRet
        {
            get
            {
                return tenderGiftCardRetField;
            }
            set
            {
                if (((tenderGiftCardRetField == null)
                            || (tenderGiftCardRetField.Equals(value) != true)))
                {
                    tenderGiftCardRetField = value;
                    OnPropertyChanged("TenderGiftCardRet", value);
                }
            }
        }

        [XmlElement(Order = 42)]
        [DataMember()]
        public SalesReceiptRetDataExtRet DataExtRet
        {
            get
            {
                return dataExtRetField;
            }
            set
            {
                if (((dataExtRetField == null)
                            || (dataExtRetField.Equals(value) != true)))
                {
                    dataExtRetField = value;
                    OnPropertyChanged("DataExtRet", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRet Deserialize(Stream s)
        {
            return ((SalesReceiptRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRet object
        /// </summary>
        public virtual SalesReceiptRet Clone()
        {
            return ((SalesReceiptRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetBillingInformation")]
    public partial class SalesReceiptRetBillingInformation : INotifyPropertyChanged
    {

        private string cityField;

        private string companyNameField;

        private string countryField;

        private string firstNameField;

        private string lastNameField;

        private string phoneField;

        private string phone2Field;

        private string phone3Field;

        private string phone4Field;

        private string postalCodeField;

        private string salutationField;

        private string stateField;

        private string streetField;

        private string street2Field;

        private string webNumberField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public string City
        {
            get
            {
                return cityField;
            }
            set
            {
                if (((cityField == null)
                            || (cityField.Equals(value) != true)))
                {
                    cityField = value;
                    OnPropertyChanged("City", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public string CompanyName
        {
            get
            {
                return companyNameField;
            }
            set
            {
                if (((companyNameField == null)
                            || (companyNameField.Equals(value) != true)))
                {
                    companyNameField = value;
                    OnPropertyChanged("CompanyName", value);
                }
            }
        }

        [XmlElement(Order = 2)]
        [DataMember()]
        public string Country
        {
            get
            {
                return countryField;
            }
            set
            {
                if (((countryField == null)
                            || (countryField.Equals(value) != true)))
                {
                    countryField = value;
                    OnPropertyChanged("Country", value);
                }
            }
        }

        [XmlElement(Order = 3)]
        [DataMember()]
        public string FirstName
        {
            get
            {
                return firstNameField;
            }
            set
            {
                if (((firstNameField == null)
                            || (firstNameField.Equals(value) != true)))
                {
                    firstNameField = value;
                    OnPropertyChanged("FirstName", value);
                }
            }
        }

        [XmlElement(Order = 4)]
        [DataMember()]
        public string LastName
        {
            get
            {
                return lastNameField;
            }
            set
            {
                if (((lastNameField == null)
                            || (lastNameField.Equals(value) != true)))
                {
                    lastNameField = value;
                    OnPropertyChanged("LastName", value);
                }
            }
        }

        [XmlElement(Order = 5)]
        [DataMember()]
        public string Phone
        {
            get
            {
                return phoneField;
            }
            set
            {
                if (((phoneField == null)
                            || (phoneField.Equals(value) != true)))
                {
                    phoneField = value;
                    OnPropertyChanged("Phone", value);
                }
            }
        }

        [XmlElement(Order = 6)]
        [DataMember()]
        public string Phone2
        {
            get
            {
                return phone2Field;
            }
            set
            {
                if (((phone2Field == null)
                            || (phone2Field.Equals(value) != true)))
                {
                    phone2Field = value;
                    OnPropertyChanged("Phone2", value);
                }
            }
        }

        [XmlElement(Order = 7)]
        [DataMember()]
        public string Phone3
        {
            get
            {
                return phone3Field;
            }
            set
            {
                if (((phone3Field == null)
                            || (phone3Field.Equals(value) != true)))
                {
                    phone3Field = value;
                    OnPropertyChanged("Phone3", value);
                }
            }
        }

        [XmlElement(Order = 8)]
        [DataMember()]
        public string Phone4
        {
            get
            {
                return phone4Field;
            }
            set
            {
                if (((phone4Field == null)
                            || (phone4Field.Equals(value) != true)))
                {
                    phone4Field = value;
                    OnPropertyChanged("Phone4", value);
                }
            }
        }

        [XmlElement(Order = 9)]
        [DataMember()]
        public string PostalCode
        {
            get
            {
                return postalCodeField;
            }
            set
            {
                if (((postalCodeField == null)
                            || (postalCodeField.Equals(value) != true)))
                {
                    postalCodeField = value;
                    OnPropertyChanged("PostalCode", value);
                }
            }
        }

        [XmlElement(Order = 10)]
        [DataMember()]
        public string Salutation
        {
            get
            {
                return salutationField;
            }
            set
            {
                if (((salutationField == null)
                            || (salutationField.Equals(value) != true)))
                {
                    salutationField = value;
                    OnPropertyChanged("Salutation", value);
                }
            }
        }

        [XmlElement(Order = 11)]
        [DataMember()]
        public string State
        {
            get
            {
                return stateField;
            }
            set
            {
                if (((stateField == null)
                            || (stateField.Equals(value) != true)))
                {
                    stateField = value;
                    OnPropertyChanged("State", value);
                }
            }
        }

        [XmlElement(Order = 12)]
        [DataMember()]
        public string Street
        {
            get
            {
                return streetField;
            }
            set
            {
                if (((streetField == null)
                            || (streetField.Equals(value) != true)))
                {
                    streetField = value;
                    OnPropertyChanged("Street", value);
                }
            }
        }

        [XmlElement(Order = 13)]
        [DataMember()]
        public string Street2
        {
            get
            {
                return street2Field;
            }
            set
            {
                if (((street2Field == null)
                            || (street2Field.Equals(value) != true)))
                {
                    street2Field = value;
                    OnPropertyChanged("Street2", value);
                }
            }
        }

        [XmlElement(Order = 14)]
        [DataMember()]
        public string WebNumber
        {
            get
            {
                return webNumberField;
            }
            set
            {
                if (((webNumberField == null)
                            || (webNumberField.Equals(value) != true)))
                {
                    webNumberField = value;
                    OnPropertyChanged("WebNumber", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetBillingInformation));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetBillingInformation object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetBillingInformation object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetBillingInformation object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetBillingInformation obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetBillingInformation);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetBillingInformation obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetBillingInformation Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetBillingInformation)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetBillingInformation Deserialize(Stream s)
        {
            return ((SalesReceiptRetBillingInformation)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetBillingInformation object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetBillingInformation object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetBillingInformation object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetBillingInformation obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetBillingInformation);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetBillingInformation obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetBillingInformation obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetBillingInformation LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetBillingInformation LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetBillingInformation object
        /// </summary>
        public virtual SalesReceiptRetBillingInformation Clone()
        {
            return ((SalesReceiptRetBillingInformation)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetShippingInformation")]
    public partial class SalesReceiptRetShippingInformation : INotifyPropertyChanged
    {

        private string addressNameField;

        private string cityField;

        private string companyNameField;

        private string countryField;

        private string fullNameField;

        private string phoneField;

        private string phone2Field;

        private string phone3Field;

        private string phone4Field;

        private string postalCodeField;

        private DateTime shipByField;

        private decimal shippingField;

        private string stateField;

        private string streetField;

        private string street2Field;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public string AddressName
        {
            get
            {
                return addressNameField;
            }
            set
            {
                if (((addressNameField == null)
                            || (addressNameField.Equals(value) != true)))
                {
                    addressNameField = value;
                    OnPropertyChanged("AddressName", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public string City
        {
            get
            {
                return cityField;
            }
            set
            {
                if (((cityField == null)
                            || (cityField.Equals(value) != true)))
                {
                    cityField = value;
                    OnPropertyChanged("City", value);
                }
            }
        }

        [XmlElement(Order = 2)]
        [DataMember()]
        public string CompanyName
        {
            get
            {
                return companyNameField;
            }
            set
            {
                if (((companyNameField == null)
                            || (companyNameField.Equals(value) != true)))
                {
                    companyNameField = value;
                    OnPropertyChanged("CompanyName", value);
                }
            }
        }

        [XmlElement(Order = 3)]
        [DataMember()]
        public string Country
        {
            get
            {
                return countryField;
            }
            set
            {
                if (((countryField == null)
                            || (countryField.Equals(value) != true)))
                {
                    countryField = value;
                    OnPropertyChanged("Country", value);
                }
            }
        }

        [XmlElement(Order = 4)]
        [DataMember()]
        public string FullName
        {
            get
            {
                return fullNameField;
            }
            set
            {
                if (((fullNameField == null)
                            || (fullNameField.Equals(value) != true)))
                {
                    fullNameField = value;
                    OnPropertyChanged("FullName", value);
                }
            }
        }

        [XmlElement(Order = 5)]
        [DataMember()]
        public string Phone
        {
            get
            {
                return phoneField;
            }
            set
            {
                if (((phoneField == null)
                            || (phoneField.Equals(value) != true)))
                {
                    phoneField = value;
                    OnPropertyChanged("Phone", value);
                }
            }
        }

        [XmlElement(Order = 6)]
        [DataMember()]
        public string Phone2
        {
            get
            {
                return phone2Field;
            }
            set
            {
                if (((phone2Field == null)
                            || (phone2Field.Equals(value) != true)))
                {
                    phone2Field = value;
                    OnPropertyChanged("Phone2", value);
                }
            }
        }

        [XmlElement(Order = 7)]
        [DataMember()]
        public string Phone3
        {
            get
            {
                return phone3Field;
            }
            set
            {
                if (((phone3Field == null)
                            || (phone3Field.Equals(value) != true)))
                {
                    phone3Field = value;
                    OnPropertyChanged("Phone3", value);
                }
            }
        }

        [XmlElement(Order = 8)]
        [DataMember()]
        public string Phone4
        {
            get
            {
                return phone4Field;
            }
            set
            {
                if (((phone4Field == null)
                            || (phone4Field.Equals(value) != true)))
                {
                    phone4Field = value;
                    OnPropertyChanged("Phone4", value);
                }
            }
        }

        [XmlElement(Order = 9)]
        [DataMember()]
        public string PostalCode
        {
            get
            {
                return postalCodeField;
            }
            set
            {
                if (((postalCodeField == null)
                            || (postalCodeField.Equals(value) != true)))
                {
                    postalCodeField = value;
                    OnPropertyChanged("PostalCode", value);
                }
            }
        }

        [XmlElement(DataType = "date", Order = 10)]
        [DataMember()]
        public DateTime ShipBy
        {
            get
            {
                return shipByField;
            }
            set
            {
                if ((shipByField.Equals(value) != true))
                {
                    shipByField = value;
                    OnPropertyChanged("ShipBy", value);
                }
            }
        }

        [XmlElement(Order = 11)]
        [DataMember()]
        public decimal Shipping
        {
            get
            {
                return shippingField;
            }
            set
            {
                if ((shippingField.Equals(value) != true))
                {
                    shippingField = value;
                    OnPropertyChanged("Shipping", value);
                }
            }
        }

        [XmlElement(Order = 12)]
        [DataMember()]
        public string State
        {
            get
            {
                return stateField;
            }
            set
            {
                if (((stateField == null)
                            || (stateField.Equals(value) != true)))
                {
                    stateField = value;
                    OnPropertyChanged("State", value);
                }
            }
        }

        [XmlElement(Order = 13)]
        [DataMember()]
        public string Street
        {
            get
            {
                return streetField;
            }
            set
            {
                if (((streetField == null)
                            || (streetField.Equals(value) != true)))
                {
                    streetField = value;
                    OnPropertyChanged("Street", value);
                }
            }
        }

        [XmlElement(Order = 14)]
        [DataMember()]
        public string Street2
        {
            get
            {
                return street2Field;
            }
            set
            {
                if (((street2Field == null)
                            || (street2Field.Equals(value) != true)))
                {
                    street2Field = value;
                    OnPropertyChanged("Street2", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetShippingInformation));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetShippingInformation object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetShippingInformation object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetShippingInformation object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetShippingInformation obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetShippingInformation);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetShippingInformation obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetShippingInformation Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetShippingInformation)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetShippingInformation Deserialize(Stream s)
        {
            return ((SalesReceiptRetShippingInformation)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetShippingInformation object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetShippingInformation object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetShippingInformation object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetShippingInformation obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetShippingInformation);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetShippingInformation obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetShippingInformation obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetShippingInformation LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetShippingInformation LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetShippingInformation object
        /// </summary>
        public virtual SalesReceiptRetShippingInformation Clone()
        {
            return ((SalesReceiptRetShippingInformation)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetSalesReceiptItemRet")]
    public partial class SalesReceiptRetSalesReceiptItemRet : INotifyPropertyChanged
    {

        private string listIDField;

        private string aLUField;

        private string associateField;

        private string attributeField;

        private decimal commissionField;

        private decimal costField;

        private string desc1Field;

        private string desc2Field;

        private decimal discountField;

        private decimal discountPercentField;

        private string discountTypeField;

        private string discountSourceField;

        private decimal extendedPriceField;

        private decimal extendedTaxField;

        private string itemNumberField;

        private string numberOfBaseUnitsField;

        private decimal priceField;

        private string priceLevelNumberField;

        private string qtyField;

        private string serialNumberField;

        private string sizeField;

        private decimal taxAmountField;

        private string taxCodeField;

        private decimal taxPercentageField;

        private string unitOfMeasureField;

        private string uPCField;

        private string webDescField;

        private string manufacturerField;

        private decimal weightField;

        private string webSKUField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public string ListID
        {
            get
            {
                return listIDField;
            }
            set
            {
                if (((listIDField == null)
                            || (listIDField.Equals(value) != true)))
                {
                    listIDField = value;
                    OnPropertyChanged("ListID", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public string ALU
        {
            get
            {
                return aLUField;
            }
            set
            {
                if (((aLUField == null)
                            || (aLUField.Equals(value) != true)))
                {
                    aLUField = value;
                    OnPropertyChanged("ALU", value);
                }
            }
        }

        [XmlElement(Order = 2)]
        [DataMember()]
        public string Associate
        {
            get
            {
                return associateField;
            }
            set
            {
                if (((associateField == null)
                            || (associateField.Equals(value) != true)))
                {
                    associateField = value;
                    OnPropertyChanged("Associate", value);
                }
            }
        }

        [XmlElement(Order = 3)]
        [DataMember()]
        public string Attribute
        {
            get
            {
                return attributeField;
            }
            set
            {
                if (((attributeField == null)
                            || (attributeField.Equals(value) != true)))
                {
                    attributeField = value;
                    OnPropertyChanged("Attribute", value);
                }
            }
        }

        [XmlElement(Order = 4)]
        [DataMember()]
        public decimal Commission
        {
            get
            {
                return commissionField;
            }
            set
            {
                if ((commissionField.Equals(value) != true))
                {
                    commissionField = value;
                    OnPropertyChanged("Commission", value);
                }
            }
        }

        [XmlElement(Order = 5)]
        [DataMember()]
        public decimal Cost
        {
            get
            {
                return costField;
            }
            set
            {
                if ((costField.Equals(value) != true))
                {
                    costField = value;
                    OnPropertyChanged("Cost", value);
                }
            }
        }

        [XmlElement(Order = 6)]
        [DataMember()]
        public string Desc1
        {
            get
            {
                return desc1Field;
            }
            set
            {
                if (((desc1Field == null)
                            || (desc1Field.Equals(value) != true)))
                {
                    desc1Field = value;
                    OnPropertyChanged("Desc1", value);
                }
            }
        }

        [XmlElement(Order = 7)]
        [DataMember()]
        public string Desc2
        {
            get
            {
                return desc2Field;
            }
            set
            {
                if (((desc2Field == null)
                            || (desc2Field.Equals(value) != true)))
                {
                    desc2Field = value;
                    OnPropertyChanged("Desc2", value);
                }
            }
        }

        [XmlElement(Order = 8)]
        [DataMember()]
        public decimal Discount
        {
            get
            {
                return discountField;
            }
            set
            {
                if ((discountField.Equals(value) != true))
                {
                    discountField = value;
                    OnPropertyChanged("Discount", value);
                }
            }
        }

        [XmlElement(Order = 9)]
        [DataMember()]
        public decimal DiscountPercent
        {
            get
            {
                return discountPercentField;
            }
            set
            {
                if ((discountPercentField.Equals(value) != true))
                {
                    discountPercentField = value;
                    OnPropertyChanged("DiscountPercent", value);
                }
            }
        }

        [XmlElement(Order = 10)]
        [DataMember()]
        public string DiscountType
        {
            get
            {
                return discountTypeField;
            }
            set
            {
                if (((discountTypeField == null)
                            || (discountTypeField.Equals(value) != true)))
                {
                    discountTypeField = value;
                    OnPropertyChanged("DiscountType", value);
                }
            }
        }

        [XmlElement(Order = 11)]
        [DataMember()]
        public string DiscountSource
        {
            get
            {
                return discountSourceField;
            }
            set
            {
                if (((discountSourceField == null)
                            || (discountSourceField.Equals(value) != true)))
                {
                    discountSourceField = value;
                    OnPropertyChanged("DiscountSource", value);
                }
            }
        }

        [XmlElement(Order = 12)]
        [DataMember()]
        public decimal ExtendedPrice
        {
            get
            {
                return extendedPriceField;
            }
            set
            {
                if ((extendedPriceField.Equals(value) != true))
                {
                    extendedPriceField = value;
                    OnPropertyChanged("ExtendedPrice", value);
                }
            }
        }

        [XmlElement(Order = 13)]
        [DataMember()]
        public decimal ExtendedTax
        {
            get
            {
                return extendedTaxField;
            }
            set
            {
                if ((extendedTaxField.Equals(value) != true))
                {
                    extendedTaxField = value;
                    OnPropertyChanged("ExtendedTax", value);
                }
            }
        }

        [XmlElement(Order = 14)]
        [DataMember()]
        public string ItemNumber
        {
            get
            {
                return itemNumberField;
            }
            set
            {
                if (((itemNumberField == null)
                            || (itemNumberField.Equals(value) != true)))
                {
                    itemNumberField = value;
                    OnPropertyChanged("ItemNumber", value);
                }
            }
        }

        [XmlElement(Order = 15)]
        [DataMember()]
        public string NumberOfBaseUnits
        {
            get
            {
                return numberOfBaseUnitsField;
            }
            set
            {
                if (((numberOfBaseUnitsField == null)
                            || (numberOfBaseUnitsField.Equals(value) != true)))
                {
                    numberOfBaseUnitsField = value;
                    OnPropertyChanged("NumberOfBaseUnits", value);
                }
            }
        }

        [XmlElement(Order = 16)]
        [DataMember()]
        public decimal Price
        {
            get
            {
                return priceField;
            }
            set
            {
                if ((priceField.Equals(value) != true))
                {
                    priceField = value;
                    OnPropertyChanged("Price", value);
                }
            }
        }

        [XmlElement(Order = 17)]
        [DataMember()]
        public string PriceLevelNumber
        {
            get
            {
                return priceLevelNumberField;
            }
            set
            {
                if (((priceLevelNumberField == null)
                            || (priceLevelNumberField.Equals(value) != true)))
                {
                    priceLevelNumberField = value;
                    OnPropertyChanged("PriceLevelNumber", value);
                }
            }
        }

        [XmlElement(Order = 18)]
        [DataMember()]
        public string Qty
        {
            get
            {
                return qtyField;
            }
            set
            {
                if (((qtyField == null)
                            || (qtyField.Equals(value) != true)))
                {
                    qtyField = value;
                    OnPropertyChanged("Qty", value);
                }
            }
        }

        [XmlElement(Order = 19)]
        [DataMember()]
        public string SerialNumber
        {
            get
            {
                return serialNumberField;
            }
            set
            {
                if (((serialNumberField == null)
                            || (serialNumberField.Equals(value) != true)))
                {
                    serialNumberField = value;
                    OnPropertyChanged("SerialNumber", value);
                }
            }
        }

        [XmlElement(Order = 20)]
        [DataMember()]
        public string Size
        {
            get
            {
                return sizeField;
            }
            set
            {
                if (((sizeField == null)
                            || (sizeField.Equals(value) != true)))
                {
                    sizeField = value;
                    OnPropertyChanged("Size", value);
                }
            }
        }

        [XmlElement(Order = 21)]
        [DataMember()]
        public decimal TaxAmount
        {
            get
            {
                return taxAmountField;
            }
            set
            {
                if ((taxAmountField.Equals(value) != true))
                {
                    taxAmountField = value;
                    OnPropertyChanged("TaxAmount", value);
                }
            }
        }

        [XmlElement(Order = 22)]
        [DataMember()]
        public string TaxCode
        {
            get
            {
                return taxCodeField;
            }
            set
            {
                if (((taxCodeField == null)
                            || (taxCodeField.Equals(value) != true)))
                {
                    taxCodeField = value;
                    OnPropertyChanged("TaxCode", value);
                }
            }
        }

        [XmlElement(Order = 23)]
        [DataMember()]
        public decimal TaxPercentage
        {
            get
            {
                return taxPercentageField;
            }
            set
            {
                if ((taxPercentageField.Equals(value) != true))
                {
                    taxPercentageField = value;
                    OnPropertyChanged("TaxPercentage", value);
                }
            }
        }

        [XmlElement(Order = 24)]
        [DataMember()]
        public string UnitOfMeasure
        {
            get
            {
                return unitOfMeasureField;
            }
            set
            {
                if (((unitOfMeasureField == null)
                            || (unitOfMeasureField.Equals(value) != true)))
                {
                    unitOfMeasureField = value;
                    OnPropertyChanged("UnitOfMeasure", value);
                }
            }
        }

        [XmlElement(Order = 25)]
        [DataMember()]
        public string UPC
        {
            get
            {
                return uPCField;
            }
            set
            {
                if (((uPCField == null)
                            || (uPCField.Equals(value) != true)))
                {
                    uPCField = value;
                    OnPropertyChanged("UPC", value);
                }
            }
        }

        [XmlElement(Order = 26)]
        [DataMember()]
        public string WebDesc
        {
            get
            {
                return webDescField;
            }
            set
            {
                if (((webDescField == null)
                            || (webDescField.Equals(value) != true)))
                {
                    webDescField = value;
                    OnPropertyChanged("WebDesc", value);
                }
            }
        }

        [XmlElement(Order = 27)]
        [DataMember()]
        public string Manufacturer
        {
            get
            {
                return manufacturerField;
            }
            set
            {
                if (((manufacturerField == null)
                            || (manufacturerField.Equals(value) != true)))
                {
                    manufacturerField = value;
                    OnPropertyChanged("Manufacturer", value);
                }
            }
        }

        [XmlElement(Order = 28)]
        [DataMember()]
        public decimal Weight
        {
            get
            {
                return weightField;
            }
            set
            {
                if ((weightField.Equals(value) != true))
                {
                    weightField = value;
                    OnPropertyChanged("Weight", value);
                }
            }
        }

        [XmlElement(Order = 29)]
        [DataMember()]
        public string WebSKU
        {
            get
            {
                return webSKUField;
            }
            set
            {
                if (((webSKUField == null)
                            || (webSKUField.Equals(value) != true)))
                {
                    webSKUField = value;
                    OnPropertyChanged("WebSKU", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetSalesReceiptItemRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetSalesReceiptItemRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetSalesReceiptItemRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetSalesReceiptItemRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetSalesReceiptItemRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetSalesReceiptItemRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetSalesReceiptItemRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetSalesReceiptItemRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetSalesReceiptItemRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetSalesReceiptItemRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetSalesReceiptItemRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetSalesReceiptItemRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetSalesReceiptItemRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetSalesReceiptItemRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetSalesReceiptItemRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetSalesReceiptItemRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetSalesReceiptItemRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetSalesReceiptItemRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetSalesReceiptItemRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetSalesReceiptItemRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetSalesReceiptItemRet object
        /// </summary>
        public virtual SalesReceiptRetSalesReceiptItemRet Clone()
        {
            return ((SalesReceiptRetSalesReceiptItemRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderAccountRet")]
    public partial class SalesReceiptRetTenderAccountRet : INotifyPropertyChanged
    {

        private decimal tenderAmountField;

        private decimal tipAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public decimal TipAmount
        {
            get
            {
                return tipAmountField;
            }
            set
            {
                if ((tipAmountField.Equals(value) != true))
                {
                    tipAmountField = value;
                    OnPropertyChanged("TipAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderAccountRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderAccountRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderAccountRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderAccountRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderAccountRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderAccountRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderAccountRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderAccountRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderAccountRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderAccountRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderAccountRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderAccountRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderAccountRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderAccountRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderAccountRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderAccountRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderAccountRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderAccountRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderAccountRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderAccountRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderAccountRet object
        /// </summary>
        public virtual SalesReceiptRetTenderAccountRet Clone()
        {
            return ((SalesReceiptRetTenderAccountRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderCashRet")]
    public partial class SalesReceiptRetTenderCashRet : INotifyPropertyChanged
    {

        private decimal tenderAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderCashRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderCashRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderCashRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderCashRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderCashRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderCashRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderCashRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderCashRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderCashRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderCashRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderCashRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderCashRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderCashRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderCashRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderCashRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderCashRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderCashRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderCashRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderCashRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderCashRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderCashRet object
        /// </summary>
        public virtual SalesReceiptRetTenderCashRet Clone()
        {
            return ((SalesReceiptRetTenderCashRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderCheckRet")]
    public partial class SalesReceiptRetTenderCheckRet : INotifyPropertyChanged
    {

        private string checkNumberField;

        private decimal tenderAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public string CheckNumber
        {
            get
            {
                return checkNumberField;
            }
            set
            {
                if (((checkNumberField == null)
                            || (checkNumberField.Equals(value) != true)))
                {
                    checkNumberField = value;
                    OnPropertyChanged("CheckNumber", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderCheckRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderCheckRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderCheckRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderCheckRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderCheckRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderCheckRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderCheckRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderCheckRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderCheckRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderCheckRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderCheckRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderCheckRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderCheckRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderCheckRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderCheckRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderCheckRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderCheckRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderCheckRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderCheckRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderCheckRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderCheckRet object
        /// </summary>
        public virtual SalesReceiptRetTenderCheckRet Clone()
        {
            return ((SalesReceiptRetTenderCheckRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderCreditCardRet")]
    public partial class SalesReceiptRetTenderCreditCardRet : INotifyPropertyChanged
    {

        private string cardNameField;

        private decimal tenderAmountField;

        private decimal tipAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public string CardName
        {
            get
            {
                return cardNameField;
            }
            set
            {
                if (((cardNameField == null)
                            || (cardNameField.Equals(value) != true)))
                {
                    cardNameField = value;
                    OnPropertyChanged("CardName", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        [XmlElement(Order = 2)]
        [DataMember()]
        public decimal TipAmount
        {
            get
            {
                return tipAmountField;
            }
            set
            {
                if ((tipAmountField.Equals(value) != true))
                {
                    tipAmountField = value;
                    OnPropertyChanged("TipAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderCreditCardRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderCreditCardRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderCreditCardRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderCreditCardRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderCreditCardRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderCreditCardRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderCreditCardRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderCreditCardRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderCreditCardRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderCreditCardRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderCreditCardRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderCreditCardRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderCreditCardRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderCreditCardRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderCreditCardRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderCreditCardRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderCreditCardRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderCreditCardRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderCreditCardRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderCreditCardRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderCreditCardRet object
        /// </summary>
        public virtual SalesReceiptRetTenderCreditCardRet Clone()
        {
            return ((SalesReceiptRetTenderCreditCardRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderDebitCardRet")]
    public partial class SalesReceiptRetTenderDebitCardRet : INotifyPropertyChanged
    {

        private decimal cashbackField;

        private decimal tenderAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public decimal Cashback
        {
            get
            {
                return cashbackField;
            }
            set
            {
                if ((cashbackField.Equals(value) != true))
                {
                    cashbackField = value;
                    OnPropertyChanged("Cashback", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderDebitCardRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderDebitCardRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderDebitCardRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderDebitCardRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderDebitCardRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderDebitCardRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderDebitCardRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderDebitCardRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderDebitCardRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderDebitCardRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderDebitCardRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderDebitCardRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderDebitCardRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderDebitCardRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderDebitCardRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderDebitCardRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderDebitCardRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderDebitCardRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderDebitCardRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderDebitCardRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderDebitCardRet object
        /// </summary>
        public virtual SalesReceiptRetTenderDebitCardRet Clone()
        {
            return ((SalesReceiptRetTenderDebitCardRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderDepositRet")]
    public partial class SalesReceiptRetTenderDepositRet : INotifyPropertyChanged
    {

        private decimal tenderAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderDepositRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderDepositRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderDepositRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderDepositRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderDepositRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderDepositRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderDepositRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderDepositRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderDepositRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderDepositRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderDepositRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderDepositRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderDepositRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderDepositRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderDepositRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderDepositRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderDepositRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderDepositRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderDepositRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderDepositRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderDepositRet object
        /// </summary>
        public virtual SalesReceiptRetTenderDepositRet Clone()
        {
            return ((SalesReceiptRetTenderDepositRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderGiftRet")]
    public partial class SalesReceiptRetTenderGiftRet : INotifyPropertyChanged
    {

        private string giftCertificateNumberField;

        private decimal tenderAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public string GiftCertificateNumber
        {
            get
            {
                return giftCertificateNumberField;
            }
            set
            {
                if (((giftCertificateNumberField == null)
                            || (giftCertificateNumberField.Equals(value) != true)))
                {
                    giftCertificateNumberField = value;
                    OnPropertyChanged("GiftCertificateNumber", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderGiftRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderGiftRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderGiftRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderGiftRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderGiftRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderGiftRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderGiftRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderGiftRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderGiftRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderGiftRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderGiftRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderGiftRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderGiftRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderGiftRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderGiftRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderGiftRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderGiftRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderGiftRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderGiftRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderGiftRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderGiftRet object
        /// </summary>
        public virtual SalesReceiptRetTenderGiftRet Clone()
        {
            return ((SalesReceiptRetTenderGiftRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetTenderGiftCardRet")]
    public partial class SalesReceiptRetTenderGiftCardRet : INotifyPropertyChanged
    {

        private decimal tenderAmountField;

        private decimal tipAmountField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public decimal TenderAmount
        {
            get
            {
                return tenderAmountField;
            }
            set
            {
                if ((tenderAmountField.Equals(value) != true))
                {
                    tenderAmountField = value;
                    OnPropertyChanged("TenderAmount", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public decimal TipAmount
        {
            get
            {
                return tipAmountField;
            }
            set
            {
                if ((tipAmountField.Equals(value) != true))
                {
                    tipAmountField = value;
                    OnPropertyChanged("TipAmount", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetTenderGiftCardRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetTenderGiftCardRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetTenderGiftCardRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderGiftCardRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetTenderGiftCardRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderGiftCardRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetTenderGiftCardRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetTenderGiftCardRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetTenderGiftCardRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetTenderGiftCardRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetTenderGiftCardRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetTenderGiftCardRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetTenderGiftCardRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetTenderGiftCardRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetTenderGiftCardRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetTenderGiftCardRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderGiftCardRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetTenderGiftCardRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetTenderGiftCardRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetTenderGiftCardRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetTenderGiftCardRet object
        /// </summary>
        public virtual SalesReceiptRetTenderGiftCardRet Clone()
        {
            return ((SalesReceiptRetTenderGiftCardRet)(MemberwiseClone()));
        }
        #endregion
    }

    [GeneratedCode("System.Xml", "4.0.30319.17929")]
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [DataContract(Name = "SalesReceiptRetDataExtRet")]
    public partial class SalesReceiptRetDataExtRet : INotifyPropertyChanged
    {

        private string ownerIDField;

        private string dataExtNameField;

        private string dataExtTypeField;

        private string dataExtValueField;

        private static XmlSerializer serializer;

        private ObjectChangeTracker changeTrackerField;

        [XmlElement(Order = 0)]
        [DataMember()]
        public string OwnerID
        {
            get
            {
                return ownerIDField;
            }
            set
            {
                if (((ownerIDField == null)
                            || (ownerIDField.Equals(value) != true)))
                {
                    ownerIDField = value;
                    OnPropertyChanged("OwnerID", value);
                }
            }
        }

        [XmlElement(Order = 1)]
        [DataMember()]
        public string DataExtName
        {
            get
            {
                return dataExtNameField;
            }
            set
            {
                if (((dataExtNameField == null)
                            || (dataExtNameField.Equals(value) != true)))
                {
                    dataExtNameField = value;
                    OnPropertyChanged("DataExtName", value);
                }
            }
        }

        [XmlElement(Order = 2)]
        [DataMember()]
        public string DataExtType
        {
            get
            {
                return dataExtTypeField;
            }
            set
            {
                if (((dataExtTypeField == null)
                            || (dataExtTypeField.Equals(value) != true)))
                {
                    dataExtTypeField = value;
                    OnPropertyChanged("DataExtType", value);
                }
            }
        }

        [XmlElement(Order = 3)]
        [DataMember()]
        public string DataExtValue
        {
            get
            {
                return dataExtValueField;
            }
            set
            {
                if (((dataExtValueField == null)
                            || (dataExtValueField.Equals(value) != true)))
                {
                    dataExtValueField = value;
                    OnPropertyChanged("DataExtValue", value);
                }
            }
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializer(typeof(SalesReceiptRetDataExtRet));
                }
                return serializer;
            }
        }

        [XmlIgnore()]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if ((changeTrackerField == null))
                {
                    changeTrackerField = new ObjectChangeTracker(this);
                }
                return changeTrackerField;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName, object value)
        {
            ChangeTracker.RecordCurrentValue(propertyName, value);
            var handler = PropertyChanged;
            if ((handler != null))
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current SalesReceiptRetDataExtRet object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize(Encoding encoding)
        {
            StreamReader streamReader = null;
            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Encoding = encoding;
                var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
                Serializer.Serialize(xmlWriter, this);
                memoryStream.Seek(0, SeekOrigin.Begin);
                streamReader = new StreamReader(memoryStream, encoding);
                return streamReader.ReadToEnd();
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        /// <summary>
        /// Deserializes workflow markup into an SalesReceiptRetDataExtRet object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output SalesReceiptRetDataExtRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out SalesReceiptRetDataExtRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetDataExtRet);
            try
            {
                obj = Deserialize(xml);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool Deserialize(string xml, out SalesReceiptRetDataExtRet obj)
        {
            Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }

        public static SalesReceiptRetDataExtRet Deserialize(string xml)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(xml);
                return ((SalesReceiptRetDataExtRet)(Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static SalesReceiptRetDataExtRet Deserialize(Stream s)
        {
            return ((SalesReceiptRetDataExtRet)(Serializer.Deserialize(s)));
        }

        /// <summary>
        /// Serializes current SalesReceiptRetDataExtRet object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, Encoding encoding, out Exception exception)
        {
            exception = null;
            try
            {
                SaveToFile(fileName, encoding);
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public virtual bool SaveToFile(string fileName, out Exception exception)
        {
            return SaveToFile(fileName, Encoding.UTF8, out exception);
        }

        public virtual void SaveToFile(string fileName)
        {
            SaveToFile(fileName, Encoding.UTF8);
        }

        public virtual void SaveToFile(string fileName, Encoding encoding)
        {
            StreamWriter streamWriter = null;
            try
            {
                var xmlString = Serialize(encoding);
                streamWriter = new StreamWriter(fileName, false, Encoding.UTF8);
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally
            {
                if ((streamWriter != null))
                {
                    streamWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// Deserializes xml markup from file into an SalesReceiptRetDataExtRet object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output SalesReceiptRetDataExtRet object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, Encoding encoding, out SalesReceiptRetDataExtRet obj, out Exception exception)
        {
            exception = null;
            obj = default(SalesReceiptRetDataExtRet);
            try
            {
                obj = LoadFromFile(fileName, encoding);
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetDataExtRet obj, out Exception exception)
        {
            return LoadFromFile(fileName, Encoding.UTF8, out obj, out exception);
        }

        public static bool LoadFromFile(string fileName, out SalesReceiptRetDataExtRet obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static SalesReceiptRetDataExtRet LoadFromFile(string fileName)
        {
            return LoadFromFile(fileName, Encoding.UTF8);
        }

        public static SalesReceiptRetDataExtRet LoadFromFile(string fileName, Encoding encoding)
        {
            FileStream file = null;
            StreamReader sr = null;
            try
            {
                file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(file, encoding);
                var xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally
            {
                if ((file != null))
                {
                    file.Dispose();
                }
                if ((sr != null))
                {
                    sr.Dispose();
                }
            }
        }
        #endregion

        #region Clone method
        /// <summary>
        /// Create a clone of this SalesReceiptRetDataExtRet object
        /// </summary>
        public virtual SalesReceiptRetDataExtRet Clone()
        {
            return ((SalesReceiptRetDataExtRet)(MemberwiseClone()));
        }
        #endregion
    }

}