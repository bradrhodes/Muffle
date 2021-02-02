#include <Arduino.h>
#include <Bounce2.h>
#define onboardLed 13
#define buttonLed 4

// Setup buttons
#define NUM_BUTTONS 1
const uint8_t BUTTON_PINS[NUM_BUTTONS] = {2};
Bounce * buttons = new Bounce[NUM_BUTTONS];

// Setup LEDs
int ledState = LOW;

char c;  // used to save incoming data
String appendSerialData; // used to save data from c
unsigned long previousMilliseconds = 0;
const long interval = 500;

// Function declarations
void CheckForEvents();
void ProcessEvent(String);
void SetMuteState(bool);
void GetCurrentMuteState();
void ToggleMuteState();
void SendCommand(String);

void setup() {
  // Add serial communication
  // Setup the Buttons
  for (int i = 0; i < NUM_BUTTONS; i++){
    buttons[i].attach(BUTTON_PINS[i], INPUT_PULLUP);
    buttons[i].interval(25);
  }

  // Setup the LED
  pinMode(onboardLed, OUTPUT);
  pinMode(buttonLed, OUTPUT);
  digitalWrite(onboardLed, ledState);
  digitalWrite(buttonLed, ledState);

  // Setup the serial communication
  // Serial.begin(115200);
  Serial.begin(57600);

  Serial.println("Finished setup.");
}

void loop() {
  bool needToToggleMute = false;

  // Get the current mute state to ensure the indicator is correct
  // GetCurrentMuteState();
  unsigned long currentMilliseconds = millis();
  if(currentMilliseconds - previousMilliseconds >= interval){
    previousMilliseconds = currentMilliseconds;


  // Check for events
    CheckForEvents();
    //GetCurrentMuteState();
  }

  // Check the current state of the button
  for(int i = 0; i < NUM_BUTTONS; i++)
  {
    // Update the Bounce instance
    buttons[i].update();
    // If it fell, flag the need to toggle the LED
    if(buttons[i].fell())
    {
      Serial.println("Button pressed");
      needToToggleMute = true;
    }
  }

  // if a LED toggle has been flagged
  if (needToToggleMute) 
  {
    // Serial.print("needToToggleMute: ");
    // Serial.println(needToToggleMute ? "true" : "false");
    Serial.println("Toggling");
    ToggleMuteState();
  }
}

void CheckForEvents()
{
  String event;
  char c;
  while(Serial.available() > 0)
  {
    c = Serial.read();
    if(c != '#')
    {
      event += c;
    }
    else
    {
      ProcessEvent(event);
      event = "";
      c = 0;
    }
  }
  // if(event != "")
  // {
  //   ProcessEvent(event);
  // }
  // event = "";
  // c = 0;
}

void ProcessEvent(String event)
{
  if(event == "mutestatetrue")
  {
    SetMuteState(true);
  }
  if(event == "mutestatefalse")
  {
    SetMuteState(false);
  }
}

void SetMuteState(bool state)
{
  if(state == true)
  {
    ledState = HIGH;
  }
  else
  {
    ledState = LOW;
  }
  digitalWrite(onboardLed, ledState);
  digitalWrite(buttonLed, ledState);
} 

void GetCurrentMuteState()
{
  SendCommand("getcurrentmutestate");
}
void ToggleMuteState()
{
  // ledState = !ledState;
  // SetMuteState(ledState);
  SendCommand("togglemutestate");
}

void SendCommand(String command)
{
  Serial.println(command);
}
