﻿using System;

using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Support.V4.Content.Res;
using Android.Support.V4.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Common.Geo;

namespace Edison.Mobile.User.Client.Droid.Holders
{
    public class ResponseViewHolder : RecyclerView.ViewHolder, IOnMapReadyCallback
    {




        private View _layout;

        private float _colorHue = -1;

        private LatLng _userLocation = null;
        private LatLng _userLocationOld = null;
        private LatLng _eventLocation = null;
        private LatLng _eventLocationOld = null;
        private double _latDelta;
        private double _longDelta;
        private Marker _userLocationMarker;
        private Marker _eventLocationMarker;

        private static BitmapDescriptor _userLocationIcon = null;
        private BitmapDescriptor UserLocationIcon
        {
            get
            {
                if (_userLocationIcon== null)
                    _userLocationIcon = CreateUserIconDrawable().ToBitmapDescriptor();
                return _userLocationIcon;
            }
        }




        // Declare Content views
        public CardView Card { get; private set; }
        public View Ripple { get; private set; }
        public ProgressBar Loading { get; private set; }
        public MapView Map { get; private set; }
        public GoogleMap GMap { get; private set; }
        public View Seperator { get; private set; }
        public AppCompatImageView Icon { get; private set; }
        public AppCompatTextView Alert { get; private set; }
        public AppCompatTextView AlertTime { get; private set; }
        public AppCompatTextView AlertDescription { get; private set; }
        public AppCompatTextView Button { get; private set; }


        public ResponseViewHolder(View item, Action<int> listener) : base(item)
        {
            _layout = item;
            BindViews(item);
            Ripple.Click += (s, e) => listener(base.LayoutPosition);
            InitializeMapView();
        }


        private void BindViews(View item)
        {
            Card = item.FindViewById<CardView>(Resource.Id.card);
            Ripple = item.FindViewById<View>(Resource.Id.ripple);
            Loading = item.FindViewById<ProgressBar>(Resource.Id.card_loading);
            Map = item.FindViewById<MapView>(Resource.Id.card_map);
            Seperator = item.FindViewById<View>(Resource.Id.card_seperator);
            Icon = item.FindViewById<AppCompatImageView>(Resource.Id.card_icon);
            Alert = item.FindViewById<AppCompatTextView>(Resource.Id.card_alert);
            AlertTime = item.FindViewById<AppCompatTextView>(Resource.Id.card_alert_time);
            AlertDescription = item.FindViewById<AppCompatTextView>(Resource.Id.card_alert_description);
            Button = item.FindViewById<AppCompatTextView>(Resource.Id.more_info_btn);
        }

        private void InitializeMapView()
        {
            Map?.OnCreate(null);
            Map?.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            MapsInitializer.Initialize(Application.Context);
            GMap = googleMap;
            GMap.UiSettings.CompassEnabled = false;
            GMap.UiSettings.MyLocationButtonEnabled = false;
            GMap.UiSettings.MapToolbarEnabled = false;
            DrawMap();
 //           SetMapLocation();
        }

        /*
                private void SetMapLocation()
                {
                    if (GMap == null) return;

                    var location = Map.Tag as LatLng;
                    if (location == null && _userLocation == null) return;

                    // Calculate the map position and zoom/size
                    CameraUpdate cameraUpdate = null;
                    if (_userLocation == null)
                        cameraUpdate = CameraUpdateFactory.NewLatLngZoom(location, Constants.DefaultResponseMapZoom);
                    else if (location == null)
                        cameraUpdate = CameraUpdateFactory.NewLatLngZoom(_userLocation, Constants.DefaultResponseMapZoom);
                    else
                    {
                        var latDelta = Math.Abs(location.Latitude - _userLocation.Latitude);
                        var longDelta = Math.Abs(location.Longitude - _userLocation.Longitude);
                        var minLat = Math.Min(location.Latitude, _userLocation.Latitude) - latDelta / 4;
                        var maxLat = Math.Max(location.Latitude, _userLocation.Latitude) + latDelta / 4;
                        var minLong = Math.Min(location.Longitude, _userLocation.Longitude) - longDelta / 4;
                        var maxLong = Math.Max(location.Longitude, _userLocation.Longitude) + longDelta / 4;

                        LatLngBounds.Builder builder = new LatLngBounds.Builder();
                        builder.Include(new LatLng(minLat, minLong));
                        builder.Include(new LatLng(maxLat, maxLong));
                        // shouldn't need to include these but we'll include them just in case
                        builder.Include(new LatLng(location.Latitude, location.Longitude));
                        builder.Include(new LatLng(_userLocation.Latitude, _userLocation.Longitude));
                        LatLngBounds bounds = builder.Build();
                        cameraUpdate = CameraUpdateFactory.NewLatLngBounds(bounds, 0);
                    }

                    // Set the map position
                    GMap.MoveCamera(cameraUpdate);

                    // Add a markers
                    if (location != null)
                    {
                        var markerOptions = new MarkerOptions();
                        markerOptions.SetPosition(location);
                        if (_colorHue > -1)
                        {
                            var bmDescriptor = BitmapDescriptorFactory.DefaultMarker(_colorHue);
                            markerOptions.SetIcon(bmDescriptor);
                        }
                        GMap.AddMarker(markerOptions);
                    }
                    if (_userLocation != null)
                    {
                        var markerOptions0 = new MarkerOptions();
                        markerOptions0.SetPosition(_userLocation);
                        markerOptions0.SetIcon(UserLocationIcon);
                        markerOptions0.Anchor(0.5f, 0.5f);
                        GMap.AddMarker(markerOptions0);
                    }

                    // Set the map type back to normal.
                    GMap.MapType = GoogleMap.MapTypeNormal;
                }
        */



        private void DrawMap(bool moveMap = true)
        {
            if (GMap == null) return;

            if (_eventLocation == null && _userLocation == null) return;

            // Calculate the map position and zoom/size
            CameraUpdate cameraUpdate = null;
            if (moveMap)
            {
                if (_userLocation == null)
                    cameraUpdate = CameraUpdateFactory.NewLatLngZoom(_eventLocation, Constants.DefaultResponseMapZoom);
                else if (_eventLocation == null)
                    cameraUpdate = CameraUpdateFactory.NewLatLngZoom(_userLocation, Constants.DefaultResponseMapZoom);
                else
                {
                    _latDelta = Math.Abs(_eventLocation.Latitude - _userLocation.Latitude);
                    _longDelta = Math.Abs(_eventLocation.Longitude - _userLocation.Longitude);
                    var minLat = Math.Min(_eventLocation.Latitude, _userLocation.Latitude) - _latDelta / 4;
                    var maxLat = Math.Max(_eventLocation.Latitude, _userLocation.Latitude) + _latDelta / 4;
                    var minLong = Math.Min(_eventLocation.Longitude, _userLocation.Longitude) - _longDelta / 4;
                    var maxLong = Math.Max(_eventLocation.Longitude, _userLocation.Longitude) + _longDelta / 4;

                    LatLngBounds.Builder builder = new LatLngBounds.Builder();
                    builder.Include(new LatLng(minLat, minLong));
                    builder.Include(new LatLng(maxLat, maxLong));
                    // shouldn't need to include these but we'll include them just in case
                    builder.Include(new LatLng(_eventLocation.Latitude, _eventLocation.Longitude));
                    builder.Include(new LatLng(_userLocation.Latitude, _userLocation.Longitude));
                    LatLngBounds bounds = builder.Build();
                    cameraUpdate = CameraUpdateFactory.NewLatLngBounds(bounds, 0);
                }
                // Set the map position
                GMap.MoveCamera(cameraUpdate);
            }

            // Add a markers
            if (_eventLocation != null)
            {
                if (_eventLocationMarker == null)
                {
                    var markerOptions = new MarkerOptions();
                    markerOptions.SetPosition(_eventLocation);
                    if (_colorHue > -1)
                    {
                        var bmDescriptor = BitmapDescriptorFactory.DefaultMarker(_colorHue);
                        markerOptions.SetIcon(bmDescriptor);
                    }
                    _eventLocationMarker = GMap.AddMarker(markerOptions);
                }
                else
                {
                    _eventLocationMarker.Position = _eventLocation;
                }
            }
            if (_userLocation != null)
            {
                if (_userLocationMarker == null)
                {
                    var markerOptions0 = new MarkerOptions();
                    markerOptions0.SetPosition(_userLocation);
                    markerOptions0.SetIcon(UserLocationIcon);
                    markerOptions0.Anchor(0.5f, 0.5f);
                    _userLocationMarker = GMap.AddMarker(markerOptions0);
                }
                else
                {
                    _userLocationMarker.Position = _userLocation;
                }
            }

            // Set the map type back to normal.
            GMap.MapType = GoogleMap.MapTypeNormal;
        }



/*
        public void SetMapLocation(Color eventcolor, double userLatitude = double.MinValue, double userLongitude = double.MinValue, double latitude = double.MinValue, double longitude = double.MinValue)
        {
            if (userLatitude != double.MinValue && userLongitude != double.MinValue)
                _userLocation = new LatLng(userLatitude, userLongitude);
            if (latitude != double.MinValue && longitude != double.MinValue)
            {
                var location = eventLocation;
                _layout.Tag = this;
                Map.Tag = location;
            }

            float[] hsl = new float[3];
            ColorUtils.ColorToHSL(eventcolor, hsl);
            _colorHue = hsl[0];
            SetMapLocation();
        }
*/

        public void SetupMap(Color eventcolor, LatLng userLocation, LatLng eventLocation)
        {
            bool moveMap = true;
            if (userLocation != null)
            {
                if (_userLocation == null)
                    _userLocation = userLocation;
                else {
                    // filter out small user movements
                    var latDeltaThreshold = _latDelta * 0.05;
                    var longDeltaThreshold = _longDelta * 0.05;
                    var latDelta = Math.Abs(userLocation.Latitude - _userLocation.Latitude);
                    var longDelta = Math.Abs(userLocation.Longitude - _userLocation.Longitude);
                    if (latDelta > latDeltaThreshold || longDelta > longDeltaThreshold)
                        _userLocation = userLocation;
                }
            }

            if (eventLocation != null)
            {
                var latDeltaThreshold = _latDelta * 0.1;
                var longDeltaThreshold = _longDelta * 0.1;
                if (_eventLocationOld == null)
                    _eventLocationOld = _eventLocation;
                else
                {
                    var latDelta = Math.Abs(eventLocation.Latitude - _eventLocationOld.Latitude);
                    var longDelta = Math.Abs(eventLocation.Longitude - _eventLocationOld.Longitude);
                    if (latDelta < latDeltaThreshold & longDelta < longDeltaThreshold)
                        moveMap = false;
                    else
                        _eventLocationOld = _eventLocation;
                }
                _eventLocation = eventLocation;
            }
            float[] hsl = new float[3];
            ColorUtils.ColorToHSL(eventcolor, hsl);
            _colorHue = hsl[0];
            DrawMap(moveMap);
        }

        public void UpdateMap(LatLng userLocation)
        {
            if (userLocation != null)
            {
                // Only update the map if user has moved moved than (approx) 5% of the distance to the event
                var latDeltaThreshold = _latDelta * 0.05;
                var longDeltaThreshold = _longDelta * 0.05;
                var latDelta = Math.Abs(userLocation.Latitude - _userLocation.Latitude);
                var longDelta = Math.Abs(userLocation.Longitude - _userLocation.Longitude);
                if (_eventLocation ==  null )
                {
                    // if there is no event, only the user location, check the user has moved more than 3m to update map
                    if (latDelta * 111111 > 3 || longDelta * 111111 * Math.Cos(userLocation.Latitude) > 3)
                    {
                        bool moveMap = true;
                        if (_userLocationOld == null)
                            _userLocationOld = _userLocation;
                        else
                        {
                            if (latDelta * 111111 < 5000 || longDelta * 111111 * Math.Cos(userLocation.Latitude) < 5000)
                                moveMap = false;
                            else
                                _userLocationOld = _userLocation;
                        }
                        _userLocation = userLocation;
                        DrawMap(moveMap);
                    }
                }
                else if (latDelta > latDeltaThreshold || longDelta > longDeltaThreshold)
                {
                    bool moveMap = true;
                    latDeltaThreshold = _latDelta * 0.1;
                    longDeltaThreshold = _longDelta * 0.1;
                    if (_userLocationOld == null)
                        _userLocationOld = _userLocation;
                    else
                    {
                        latDelta = Math.Abs(userLocation.Latitude - _userLocationOld.Latitude);
                        longDelta = Math.Abs(userLocation.Longitude - _userLocationOld.Longitude);
                        if (latDelta < latDeltaThreshold & longDelta < longDeltaThreshold)
                            moveMap = false;
                        else
                            _userLocationOld = _userLocation;
                    }
                    _userLocation = userLocation;
                    DrawMap(moveMap);
                }
            }
        }


        private Drawable CircleDrawable(Color color, int intrinsicSize = -1)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            if (intrinsicSize > -1)
            {
                drw.SetIntrinsicWidth(intrinsicSize);
                drw.SetIntrinsicWidth(intrinsicSize);
            }
            drw.Paint.Color = color;
            return drw;
        }




        private Drawable CreateUserIconDrawable()
        {
            int dp16 = PixelSizeConverter.DpToPx(16);
            int dp12 = PixelSizeConverter.DpToPx(12);
            int dp4 = PixelSizeConverter.DpToPx(4);
            var fillColor = new Color(ResourcesCompat.GetColor(_layout.Resources, Resource.Color.user_location, null));
            var ringColor = new Color(ResourcesCompat.GetColor(_layout.Resources, Resource.Color.user_location_stroke, null));
            var pointColor = new Color(ResourcesCompat.GetColor(_layout.Resources, Resource.Color.user_location_center, null));
            LayerDrawable drw = new LayerDrawable(new Drawable[] {CircleDrawable(ringColor, dp16), CircleDrawable(fillColor, dp12), CircleDrawable(pointColor, dp4) } );
            drw.SetLayerSize(0, dp16, dp16);
            drw.SetLayerSize(1, dp12, dp12);
            drw.SetLayerSize(2, dp4, dp4);
            drw.SetLayerGravity(0, GravityFlags.Center);
            drw.SetLayerGravity(1, GravityFlags.Center);
            drw.SetLayerGravity(2, GravityFlags.Center);
            return drw;
        }


        public void OnLocationChanged(object s, LocationChangedEventArgs e)
        {
            var userLocation = new LatLng(e.CurrentLocation.Latitude, e.CurrentLocation.Longitude);
            UpdateMap(userLocation);
        }



    }
}