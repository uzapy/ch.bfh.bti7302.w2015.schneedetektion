﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Schneedetektion.Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="StrassenbilderMeta")]
	public partial class StrassenbilderMetaDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertCamera(Camera instance);
    partial void UpdateCamera(Camera instance);
    partial void DeleteCamera(Camera instance);
    partial void InsertImage(Image instance);
    partial void UpdateImage(Image instance);
    partial void DeleteImage(Image instance);
    partial void InsertPolygon(Polygon instance);
    partial void UpdatePolygon(Polygon instance);
    partial void DeletePolygon(Polygon instance);
    #endregion
		
		public StrassenbilderMetaDataContext() : 
				base(global::Schneedetektion.Data.Properties.Settings.Default.StrassenbilderMetaConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public StrassenbilderMetaDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public StrassenbilderMetaDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public StrassenbilderMetaDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public StrassenbilderMetaDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Camera> Cameras
		{
			get
			{
				return this.GetTable<Camera>();
			}
		}
		
		public System.Data.Linq.Table<Image> Images
		{
			get
			{
				return this.GetTable<Image>();
			}
		}
		
		public System.Data.Linq.Table<Polygon> Polygons
		{
			get
			{
				return this.GetTable<Polygon>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Cameras")]
	public partial class Camera : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _Name;
		
		private string _Site;
		
		private string _Description;
		
		private string _Comment;
		
		private string _Coordinates;
		
		private System.Nullable<double> _North;
		
		private System.Nullable<double> _East;
		
		private System.Nullable<double> _Elevation;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnSiteChanging(string value);
    partial void OnSiteChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnCommentChanging(string value);
    partial void OnCommentChanged();
    partial void OnCoordinatesChanging(string value);
    partial void OnCoordinatesChanged();
    partial void OnNorthChanging(System.Nullable<double> value);
    partial void OnNorthChanged();
    partial void OnEastChanging(System.Nullable<double> value);
    partial void OnEastChanged();
    partial void OnElevationChanging(System.Nullable<double> value);
    partial void OnElevationChanged();
    #endregion
		
		public Camera()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Site", DbType="NVarChar(50)")]
		public string Site
		{
			get
			{
				return this._Site;
			}
			set
			{
				if ((this._Site != value))
				{
					this.OnSiteChanging(value);
					this.SendPropertyChanging();
					this._Site = value;
					this.SendPropertyChanged("Site");
					this.OnSiteChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(50)")]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Comment", DbType="NVarChar(50)")]
		public string Comment
		{
			get
			{
				return this._Comment;
			}
			set
			{
				if ((this._Comment != value))
				{
					this.OnCommentChanging(value);
					this.SendPropertyChanging();
					this._Comment = value;
					this.SendPropertyChanged("Comment");
					this.OnCommentChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Coordinates", DbType="NVarChar(50)")]
		public string Coordinates
		{
			get
			{
				return this._Coordinates;
			}
			set
			{
				if ((this._Coordinates != value))
				{
					this.OnCoordinatesChanging(value);
					this.SendPropertyChanging();
					this._Coordinates = value;
					this.SendPropertyChanged("Coordinates");
					this.OnCoordinatesChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_North", DbType="Float")]
		public System.Nullable<double> North
		{
			get
			{
				return this._North;
			}
			set
			{
				if ((this._North != value))
				{
					this.OnNorthChanging(value);
					this.SendPropertyChanging();
					this._North = value;
					this.SendPropertyChanged("North");
					this.OnNorthChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_East", DbType="Float")]
		public System.Nullable<double> East
		{
			get
			{
				return this._East;
			}
			set
			{
				if ((this._East != value))
				{
					this.OnEastChanging(value);
					this.SendPropertyChanging();
					this._East = value;
					this.SendPropertyChanged("East");
					this.OnEastChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Elevation", DbType="Float")]
		public System.Nullable<double> Elevation
		{
			get
			{
				return this._Elevation;
			}
			set
			{
				if ((this._Elevation != value))
				{
					this.OnElevationChanging(value);
					this.SendPropertyChanging();
					this._Elevation = value;
					this.SendPropertyChanged("Elevation");
					this.OnElevationChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Images")]
	public partial class Image : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _Name;
		
		private string _Place;
		
		private System.DateTime _DateTime;
		
		private string _TimeZone;
		
		private System.Nullable<double> _UnixTime;
		
		private System.Nullable<short> _Snow;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnPlaceChanging(string value);
    partial void OnPlaceChanged();
    partial void OnDateTimeChanging(System.DateTime value);
    partial void OnDateTimeChanged();
    partial void OnTimeZoneChanging(string value);
    partial void OnTimeZoneChanged();
    partial void OnUnixTimeChanging(System.Nullable<double> value);
    partial void OnUnixTimeChanged();
    partial void OnSnowChanging(System.Nullable<short> value);
    partial void OnSnowChanged();
    #endregion
		
		public Image()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Place", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string Place
		{
			get
			{
				return this._Place;
			}
			set
			{
				if ((this._Place != value))
				{
					this.OnPlaceChanging(value);
					this.SendPropertyChanging();
					this._Place = value;
					this.SendPropertyChanged("Place");
					this.OnPlaceChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateTime", DbType="DateTime NOT NULL")]
		public System.DateTime DateTime
		{
			get
			{
				return this._DateTime;
			}
			set
			{
				if ((this._DateTime != value))
				{
					this.OnDateTimeChanging(value);
					this.SendPropertyChanging();
					this._DateTime = value;
					this.SendPropertyChanged("DateTime");
					this.OnDateTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TimeZone", DbType="NVarChar(5)")]
		public string TimeZone
		{
			get
			{
				return this._TimeZone;
			}
			set
			{
				if ((this._TimeZone != value))
				{
					this.OnTimeZoneChanging(value);
					this.SendPropertyChanging();
					this._TimeZone = value;
					this.SendPropertyChanged("TimeZone");
					this.OnTimeZoneChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UnixTime", DbType="Float")]
		public System.Nullable<double> UnixTime
		{
			get
			{
				return this._UnixTime;
			}
			set
			{
				if ((this._UnixTime != value))
				{
					this.OnUnixTimeChanging(value);
					this.SendPropertyChanging();
					this._UnixTime = value;
					this.SendPropertyChanged("UnixTime");
					this.OnUnixTimeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Snow", DbType="SmallInt")]
		public System.Nullable<short> Snow
		{
			get
			{
				return this._Snow;
			}
			set
			{
				if ((this._Snow != value))
				{
					this.OnSnowChanging(value);
					this.SendPropertyChanging();
					this._Snow = value;
					this.SendPropertyChanged("Snow");
					this.OnSnowChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Polygons")]
	public partial class Polygon : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private string _CameraName;
		
		private string _ImageArea;
		
		private System.Nullable<double> _ImageWidth;
		
		private System.Nullable<double> _ImageHeight;
		
		private string _PolygonPointCollection;
		
		private string _BgrSnow;
		
		private string _BgrNormal;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnCameraNameChanging(string value);
    partial void OnCameraNameChanged();
    partial void OnImageAreaChanging(string value);
    partial void OnImageAreaChanged();
    partial void OnImageWidthChanging(System.Nullable<double> value);
    partial void OnImageWidthChanged();
    partial void OnImageHeightChanging(System.Nullable<double> value);
    partial void OnImageHeightChanged();
    partial void OnPolygonPointCollectionChanging(string value);
    partial void OnPolygonPointCollectionChanged();
    partial void OnBgrSnowChanging(string value);
    partial void OnBgrSnowChanged();
    partial void OnBgrNormalChanging(string value);
    partial void OnBgrNormalChanged();
    #endregion
		
		public Polygon()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CameraName", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string CameraName
		{
			get
			{
				return this._CameraName;
			}
			set
			{
				if ((this._CameraName != value))
				{
					this.OnCameraNameChanging(value);
					this.SendPropertyChanging();
					this._CameraName = value;
					this.SendPropertyChanged("CameraName");
					this.OnCameraNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ImageArea", DbType="NVarChar(50)")]
		public string ImageArea
		{
			get
			{
				return this._ImageArea;
			}
			set
			{
				if ((this._ImageArea != value))
				{
					this.OnImageAreaChanging(value);
					this.SendPropertyChanging();
					this._ImageArea = value;
					this.SendPropertyChanged("ImageArea");
					this.OnImageAreaChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ImageWidth", DbType="Float")]
		public System.Nullable<double> ImageWidth
		{
			get
			{
				return this._ImageWidth;
			}
			set
			{
				if ((this._ImageWidth != value))
				{
					this.OnImageWidthChanging(value);
					this.SendPropertyChanging();
					this._ImageWidth = value;
					this.SendPropertyChanged("ImageWidth");
					this.OnImageWidthChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ImageHeight", DbType="Float")]
		public System.Nullable<double> ImageHeight
		{
			get
			{
				return this._ImageHeight;
			}
			set
			{
				if ((this._ImageHeight != value))
				{
					this.OnImageHeightChanging(value);
					this.SendPropertyChanging();
					this._ImageHeight = value;
					this.SendPropertyChanged("ImageHeight");
					this.OnImageHeightChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PolygonPointCollection", DbType="NVarChar(MAX)")]
		public string PolygonPointCollection
		{
			get
			{
				return this._PolygonPointCollection;
			}
			set
			{
				if ((this._PolygonPointCollection != value))
				{
					this.OnPolygonPointCollectionChanging(value);
					this.SendPropertyChanging();
					this._PolygonPointCollection = value;
					this.SendPropertyChanged("PolygonPointCollection");
					this.OnPolygonPointCollectionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BgrSnow", DbType="NVarChar(200)")]
		public string BgrSnow
		{
			get
			{
				return this._BgrSnow;
			}
			set
			{
				if ((this._BgrSnow != value))
				{
					this.OnBgrSnowChanging(value);
					this.SendPropertyChanging();
					this._BgrSnow = value;
					this.SendPropertyChanged("BgrSnow");
					this.OnBgrSnowChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BgrNormal", DbType="NVarChar(200)")]
		public string BgrNormal
		{
			get
			{
				return this._BgrNormal;
			}
			set
			{
				if ((this._BgrNormal != value))
				{
					this.OnBgrNormalChanging(value);
					this.SendPropertyChanging();
					this._BgrNormal = value;
					this.SendPropertyChanged("BgrNormal");
					this.OnBgrNormalChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591