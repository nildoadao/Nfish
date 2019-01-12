using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfish.Application.Common
{
    public class Enums
    {
        public enum Health
        {
            Critical,
            OK,
            Warning
        }

        public enum HealthRollup
        {
            Critical,
            OK,
            Warning
        }

        public enum State
        {
            Absent,
            Deferring,
            Disabled,
            Enabled,
            InTest,
            Quiesced,
            StandbyOffline,
            StandbtSpare,
            Starting,
            UnavailableOffline,
            Updating
        }
        
        public enum LocationType
        {
            Bay,
            Connector,
            Slot
        }

        public enum Orientation
        {
            BackToFront,
            BottomToTop,
            FrontToBack,
            LeftToRight,
            RightToRight,
            TopToBottom
        }

        public enum RackOffsetUnits
        {
            EIA_310,
            OpenU
        }

        public enum Reference
        {
            Bottom,
            Front,
            Left,
            Middle,
            Rear,
            Right,
            Top
        }

        public enum DurableNameFormat
        {
            EUI,
            FC_WWN,
            NAA,
            NQN,
            NSID,
            UUID,
            iQN
        }

        public enum AddressOrigin
        {
            BOOTP,
            DHCP,
            IPv4LinkLocal,
            Static
        }

        public enum AddressState
        {
            Deprecated,
            Failed,
            Preferred,
            Tentative
        }

        public enum AccountProviderType
        {
            ActiveDirectoryService,
            LDAPService,
            OEM,
            RedfishService
        }

        public enum AuthenticationType
        {
            KerberosKeytab,
            OEM,
            Token,
            UsernameAndPassword
        }

        public enum LocalAccountAuth
        {
            Disabled,
            Enabled,
            Fallback
        }

        public enum DataType
        {
            Boolean,
            Number,
            NumberArray,
            Object,
            ObjectArray,
            String,
            StringArray
        }

        public enum MapFromCondition
        {
            EQU,
            GEQ,
            GTR,
            LEQ,
            LSS,
            NEQ
        }

        public enum MapFromProperty
        {
            CurrentValue,
            DefaultValue,
            GrayOut,
            Hidden,
            LowerBound,
            MaxLength,
            ReadOnly,
            ScalarIncrement,
            UpperBound,
            WritOnly
        }

        public enum MapTerms
        {
            AND,
            OR
        }

        public enum MapToProperty
        {
            CurrentValue,
            DefaultValue,
            DisplayName,
            DisplayOrder,
            GrayOut,
            HelpText,
            Hidden,
            Immutable,
            LowerBound,
            MaxLength,
            MinLength,
            ReadOnly,
            ScalarIncrement,
            UpperBound,
            ValueExpression,
            WarningText,
            WriteOnly
        }

        public enum Type
        {
            Map
        }

        public enum Alias
        {
            BiosSetup,
            Cd,
            Diags,
            Floppy,
            Hdd,
            None,
            Pxe,
            RemoteDrive,
            SDCard,
            UefiBootNext,
            UefiHttp,
            UefiShell,
            UefiTarget,
            Usb,
            Utilities
        }

        public enum ChassisType
        {
            Blade,
            Card,
            Catridge,
            Component,
            Drawer,
            Enclousure,
            Expansion,
            IPBasedDrive,
            Module,
            Other,
            Pod,
            Rack,
            RackMount,
            Row,
            Shelf,
            Sidecar,
            Sled,
            StandAlone,
            StorageEnclousure,
            Zone
        }

        public enum IndicatorLED
        {
            Blinking,
            Lit,
            Off,
            Unknown
        }

        public enum IntrusionSensor
        {
            HardwareIntrusion,
            Normal,
            TamperingDetected
        }

        public enum IntrusionSensorReArm
        {
            Automatic,
            Manual
        }

        public enum PowerState
        {
            Off,
            On,
            PoweringOff,
            PowerinOn
        }

        public enum ResetType
        {
            ForceOff,
            ForceOn,
            ForceRestart,
            GracefulRestart,
            GracefulShutdown,
            Nmi,
            On,
            PowerCycle,
            PushPowerButton
        }

        public enum BootSourceOverrideEnabled
        {
            Continuous,
            Disabled,
            Once
        }

        public enum BootSourceOverrideMode
        {
            Legacy,
            UEFI
        }

        public enum BootSourceOverrideTarget
        {
            BiosSetup,
            Cd,
            Diags,
            Floppy,
            Hdd,
            None,
            Pxe,
            RemoteDrive,
            SDCard,
            UefiBootNext,
            UefiHttp,
            UefiTarget,
            Usb,
            Utilites
        }

        public enum HostingRoles
        {
            ApplicationServer,
            StorageServer,
            Switch
        }

        public enum InterfaceType
        {
            TCM1_0,
            TPM1_2,
            TPM2_0
        }

        public enum InterfaceTypeSelection
        {
            BiosSetting,
            FirmwareUpdate,
            None,
            OemMethod
        }

        public enum MemoryMirroring
        {
            DIMM,
            Hybrid,
            None,
            System
        }

        public enum SystenType
        {
            Composed,
            OS,
            Physical,
            PhysicallyPartitioned,
            Virtual,
            VirtuallyPartitipned
        }

        public enum TimeoutAction
        {
            None,
            OEM,
            PowerCycle,
            PowerDown,
            ResetSystem
        }

        public enum WarningAction
        {
            DiagnosticInterrupt,
            MessagingInterrupt,
            None,
            OEM,
            SCI,
            SMI
        }

        public enum TransferProtocol
        {
            CIFS,
            FTP,
            HTTP,
            HTTPS,
            NSF,
            OEM,
            SCP,
            SFTP,
            TFTP
        }

        public enum VolumeType
        {
            Mirrored = 1,
            NonRedundant = 0,
            RawDevice = -1,
            SpannedMirrors = 10,
            SpannedStripesWithParity = 50,
            StripedWithParity = 5
        }
    }
}
