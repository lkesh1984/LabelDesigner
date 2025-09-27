import 'fabric';

declare module 'fabric' {
  namespace fabric {
    interface Object {
        Id: string;
        IsDynamic?: boolean;
        CustomType: string;
        BarcodeType: string
    }
  }
}