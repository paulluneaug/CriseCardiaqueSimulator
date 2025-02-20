#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.3.0
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class AkPositioningData : global::System.IDisposable {
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkPositioningData(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkPositioningData obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }

  ~AkPositioningData() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkPositioningData(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public Ak3dData threeD { set { AkSoundEnginePINVOKE.CSharp_AkPositioningData_threeD_set(swigCPtr, Ak3dData.getCPtr(value)); } 
    get {
      global::System.IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPositioningData_threeD_get(swigCPtr);
      Ak3dData ret = (cPtr == global::System.IntPtr.Zero) ? null : new Ak3dData(cPtr, false);
      return ret;
    } 
  }

  public AkBehavioralPositioningData behavioral { set { AkSoundEnginePINVOKE.CSharp_AkPositioningData_behavioral_set(swigCPtr, AkBehavioralPositioningData.getCPtr(value)); } 
    get {
      global::System.IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkPositioningData_behavioral_get(swigCPtr);
      AkBehavioralPositioningData ret = (cPtr == global::System.IntPtr.Zero) ? null : new AkBehavioralPositioningData(cPtr, false);
      return ret;
    } 
  }

  public AkPositioningData() : this(AkSoundEnginePINVOKE.CSharp_new_AkPositioningData(), true) {
  }

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.