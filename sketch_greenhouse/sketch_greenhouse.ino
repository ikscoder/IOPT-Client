#include <DHT_U.h>
#include <Wire.h>
#include <BH1750.h>
#include <SoftwareSerial.h>
#include <DHT.h>
#include <Servo.h>

#define ESP_TX 7
#define ESP_RX 6
#define LAMP_PIN 5
#define RELAY_PIN 4
#define SERVO_PIN 3
#define DHT_PIN 2
#define WATERMETER A3
#define DHTTYPE DHT11

#define SENSOR_POLL_INTERVAL 1000
#define VENT_OPEN 20
#define VENT_CLOSE 90

DHT dht(DHT_PIN, DHTTYPE);
BH1750 lightMeter;
Servo vent;

long last_sensors_poll = 0;

// DataModel
int Temperature = 0;
int Brightness = 0;
int Humidity = 0;
bool VentState = false;
bool PumpState = false;
int LampBrightness = 0;
bool autoRand = false;

void setup() {
  // put your setup code here, to run once:
Serial.begin(115200);
  dht.begin();
  digitalWrite(WATERMETER, 1);
  lightMeter.begin();
  vent.attach(SERVO_PIN);
  pinMode(LAMP_PIN, OUTPUT);
  pinMode(RELAY_PIN, OUTPUT);

  while (! Serial);
  last_sensors_poll = millis();
  //delay(2000);
  //Serial.println("Start");
}

void loop() {
 localWork();
}

void localWork()
{
  if ((millis() - last_sensors_poll) > SENSOR_POLL_INTERVAL) {
  String buf = "";
  while (Serial.available())
  {
    char c = Serial.read();
    buf += c;
  }
  
  if (buf.lastIndexOf("autooff") >= 0)autoRand=false;

  if (buf.lastIndexOf("autoon") >= 0||autoRand)
{
  autoRand=true;
  autoSwitch();
}
else{
//  if (buf.lastIndexOf("led=") >= 0) {
//    String tled = buf.substring(buf.lastIndexOf("led=") + 4);
//    tled = tled.substring(0, tled.length() - 2);
//    LampBrightness = tled.toInt();
//  }
//  if (buf.indexOf("pumpon") >= 0)PumpState=true;
//  if (buf.indexOf("pumpoff") >= 0)PumpState=false;
//  if (buf.lastIndexOf("venton") > buf.lastIndexOf("ventoff")) VentState=true;
//  if (buf.lastIndexOf("ventoff") > buf.lastIndexOf("venton")) VentState=false;
  //if (buf.indexOf("terminator") >= 0) scriptTerminator();
}



    
    Humidity = analogRead(WATERMETER);
    Temperature = dht.readTemperature();
    Brightness = lightMeter.readLightLevel();
    if (!isnan(Temperature)) {
      Serial.print(Temperature);
    } else {
      Serial.print("--");
    }
    Serial.print(";");
    Serial.print(Brightness);
    Serial.print(";");
    Serial.print(Humidity);
    Serial.print(";");
    Serial.print(VentState);
//    Serial.print(";");
//    Serial.println(PumpState);
    Serial.print(";");
    Serial.println(LampBrightness);
    last_sensors_poll = millis();

      writeOnAct();
  }

}

void scriptTerminator()
{
  for(int i=0;i<10;i++){
  vent.write(VENT_OPEN);
delay(1000);
   vent.write(VENT_CLOSE);
delay(1000);
  }
}

int ventInterval=millis();
int lampInterval=millis();
void autoSwitch()
{
   if ((millis() - ventInterval) > 2900)
   { 
  VentState=!VentState;
   ventInterval=millis();
   } 
     if ((millis() - lampInterval) > 500){ 
  LampBrightness=LampBrightness>10?0:255;
     lampInterval=millis();
     }
}

void writeOnAct()
{
  analogWrite(LAMP_PIN, LampBrightness);
   vent.write(VentState?VENT_OPEN:VENT_CLOSE);
   digitalWrite(RELAY_PIN, PumpState?1:0);
   delay(2000);
}
