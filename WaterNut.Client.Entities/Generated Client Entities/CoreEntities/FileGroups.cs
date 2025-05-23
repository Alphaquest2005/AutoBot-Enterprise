﻿// <autogenerated>
//   This file was generated by T4 code generator AllClientEntities.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>


using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using TrackableEntities.Client;
using Core.Common.Client.Entities;
using CoreEntities.Client.DTO;


using Core.Common.Validation;

namespace CoreEntities.Client.Entities
{
       public partial class FileGroups: BaseEntity<FileGroups>
    {
        DTO.FileGroups filegroups;
        public FileGroups(DTO.FileGroups dto )
        {
              filegroups = dto;
             _changeTracker = new ChangeTrackingCollection<DTO.FileGroups>(filegroups);

        }

        public DTO.FileGroups DTO
        {
            get
            {
             return filegroups;
            }
            set
            {
                filegroups = value;
            }
        }
        


       [RequiredValidationAttribute(ErrorMessage= " is required")]
       
public int Id
		{ 
		    get { return this.filegroups.Id; }
			set
			{
			    if (value == this.filegroups.Id) return;
				this.filegroups.Id = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Id");
			}
		}
     

       [RequiredValidationAttribute(ErrorMessage= "Name is required")]
       
                
                [MaxLength(50, ErrorMessage = "Name has a max length of 50 letters ")]
public string Name
		{ 
		    get { return this.filegroups.Name; }
			set
			{
			    if (value == this.filegroups.Name) return;
				this.filegroups.Name = value;
                if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				NotifyPropertyChanged("Name");
			}
		}
     

        ObservableCollection<FileTypes> _FileTypes = null;
        public  ObservableCollection<FileTypes> FileTypes
		{
            
		    get 
				{ 
					if(_FileTypes != null) return _FileTypes;
					//if (this.filegroups.FileTypes == null) Debugger.Break();
					if(this.filegroups.FileTypes != null)
					{
						_FileTypes = new ObservableCollection<FileTypes>(this.filegroups.FileTypes.Select(x => new FileTypes(x)));
					}
					
						_FileTypes.CollectionChanged += FileTypes_CollectionChanged; 
					
					return _FileTypes; 
				}
			set
			{
			    if (Equals(value, _FileTypes)) return;
				if (value != null)
					this.filegroups.FileTypes = new ChangeTrackingCollection<DTO.FileTypes>(value.Select(x => x.DTO).ToList());
                _FileTypes = value;
				if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
				if (_FileTypes != null)
				_FileTypes.CollectionChanged += FileTypes_CollectionChanged;               
				NotifyPropertyChanged("FileTypes");
			}
		}
        
        void FileTypes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (FileTypes itm in e.NewItems)
                    {
                        if (itm != null)
                        filegroups.FileTypes.Add(itm.DTO);
                    }
                    if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (FileTypes itm in e.OldItems)
                    {
                        if (itm != null)
                        filegroups.FileTypes.Remove(itm.DTO);
                    }
					if(this.TrackingState == TrackableEntities.TrackingState.Unchanged)this.TrackingState = TrackableEntities.TrackingState.Modified;
                    break;
                
            }
        }


        ChangeTrackingCollection<DTO.FileGroups> _changeTracker;    
        public ChangeTrackingCollection<DTO.FileGroups> ChangeTracker
        {
            get
            {
                return _changeTracker;
            }
        }

        public new TrackableEntities.TrackingState TrackingState
        {
            get
            {
                return this.DTO.TrackingState;
            }
            set
            {
                this.DTO.TrackingState = value;
                NotifyPropertyChanged("TrackingState");
            }
        }

    }
}


