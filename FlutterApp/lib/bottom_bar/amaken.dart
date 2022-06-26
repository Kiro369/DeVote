import 'package:flutter/material.dart';
import 'package:animated_text_kit/animated_text_kit.dart';
import 'package:geolocator/geolocator.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

class Amaken extends StatefulWidget {
  const Amaken({Key? key}) : super(key: key);

  @override
  _AmakenState createState() => _AmakenState();
}

class _AmakenState extends State<Amaken> {
  bool _mapLoading = true;


  final Set<Marker> markers = {}; //markers for google map
  static const LatLng showLocation = LatLng(30.033333, 31.233334);
  var locations = [
    {
      'name': 'المعهد التكنولوجي العالي بالعاشر من رمضان',
      'city': 'محافظة الشرقية',
      'location': const LatLng(30.286596, 31.740078)
    },
  ];

  Set<Marker> getmarkers() {
    //markers to place on map
    for (var i = 0; i < locations.length; i++) {
      final city = locations[i]['city'];
      final title = locations[i]['name'];
      final loc = locations[i]['location'];
      markers.add(Marker(
        //add first marker
        markerId: MarkerId(showLocation.toString()),
        position: loc as LatLng, //position of marker
        infoWindow: InfoWindow(
          //popup info
          title: city.toString(),
          snippet: title.toString(),
        ),

        icon: BitmapDescriptor.defaultMarker, //Icon for Marker
      ));
    }
    return markers;
  }
  GoogleMapController? mapController;
   LatLng currentPostion = const LatLng(30.286596, 31.740078);
  late LocationPermission permission;

  void _getUserLocation() async {
    permission = await Geolocator.requestPermission();
    var position = await GeolocatorPlatform.instance.getCurrentPosition(
        locationSettings:
            const LocationSettings(accuracy: LocationAccuracy.high));

    setState(() {
      currentPostion = LatLng(position.latitude, position.longitude);
    });
    mapController?.animateCamera(
        CameraUpdate.newCameraPosition(
            CameraPosition(target: currentPostion, zoom: 14)
          //17 is new zoom level
        )
    );

  }


  @override
  Widget build(BuildContext context) {
    Size size = MediaQuery.of(context).size;

    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        actions: const [
          Padding(
            padding: EdgeInsets.only(right: 10),
            child: Icon(
              Icons.check_box_outlined,
              color: Colors.white,
              size: 30,
            ),
          )
        ],
        centerTitle: true,
        title: const Text(
          'مواقع الانتخابات',
          style: TextStyle(color: Colors.white),
        ),
        backgroundColor: const Color(0xff26375f),
        bottom: PreferredSize(
          preferredSize: const Size.fromHeight(40.0),
          child: Container(
            width: MediaQuery.of(context).size.width,
            color: const Color(0xfff1f2f4),
            height: 40.0,
            child: DefaultTextStyle(
              textAlign: TextAlign.right,
              style: const TextStyle(
                fontSize: 16.0,
                color: Color(0xff000000),
              ),
              child: AnimatedTextKit(
                repeatForever: true,
                animatedTexts: [
                  RotateAnimatedText('موعد الانتخابات الرئاسية 18-19 يونيو'),
                  RotateAnimatedText(
                      'تتواجد ماكينات الانتخابات في جميع انحاء الجمهورية'),
                  RotateAnimatedText(
                      'يرجي الذهاب للانتخابات ببطاقة الرقم القومي'),
                ],
                isRepeatingAnimation: true,
              ),
            ),
          ),
        ),
      ),
      body: SizedBox(
        height: size.height,
        width: size.width,
        child: Stack(
          children: [
            Center(
              child: GoogleMap(
                markers: getmarkers(),
                // myLocationButtonEnabled: true,
                //myLocationEnabled: true,

                initialCameraPosition:  CameraPosition(
                  target: currentPostion,
                  zoom: 14,
                ),
                onMapCreated: (controller) { //method called when map is created
                setState(() {
                mapController = controller;
                _mapLoading = false;
                });
              },
              ),
            ),
            (_mapLoading)
                ? Container(
                    height: size.height,
                    width: size.width,
                    color: Colors.grey[100],
                    child: const Center(
                      child: CircularProgressIndicator(),
                    ),
                  )
                : Container(
                    alignment: Alignment.topRight,
                    child: Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: FloatingActionButton(
                      onPressed: _getUserLocation,
                      child: const Icon(Icons.my_location,color:Color(0xff26375f)),
                      backgroundColor: Colors.white,
                      elevation: 0.5,
                    )),
                  ),
          ],
        ),

      ),
    );
  }
}
