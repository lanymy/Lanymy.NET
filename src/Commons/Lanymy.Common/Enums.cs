using Lanymy.Common.ConstKeys;
using Lanymy.Common.CustomAttributes;

namespace Lanymy.Common
{
    /// <summary>
    /// 注册表根项枚举
    /// </summary>
    public enum RegeditRootEnum
    {
        HKEY_CLASSES_ROOT,
        HKEY_CURRENT_USER,
        HKEY_LOCAL_MACHINE,
        HKEY_USERS,
        HKEY_CURRENT_CONFIG,
    }


    /// <summary>
    /// 路径类别枚举
    /// </summary>
    public enum PathTypeEnum
    {
        /// <summary>
        /// 未知路径
        /// </summary>
        UnKnow,
        /// <summary>
        /// 文件路径
        /// </summary>
        File,
        /// <summary>
        /// 文件夹路径
        /// </summary>
        Directory,

    }


    /// <summary>
    /// 日期季度枚举
    /// </summary>
    public enum DateQuarterEnum
    {

        /// <summary>
        /// 第一季度
        /// </summary>
        FirstQuarter = 1,

        /// <summary>
        /// 第二季度
        /// </summary>
        SecondQuarter = 4,

        /// <summary>
        /// 第三季度
        /// </summary>
        ThirdQuarter = 7,

        /// <summary>
        /// 第四季度
        /// </summary>
        FourthQuarter = 10,

    }



    /// <summary>
    /// 日志消息类别枚举
    /// </summary>
    public enum LogMessageTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,

        /// <summary>
        /// 所有级别
        /// </summary>
        All,

        /// <summary>
        /// 堆栈消息
        /// </summary>
        Trace,

        /// <summary>
        /// 测试消息
        /// </summary>
        Debug,

        /// <summary>
        /// 内容消息
        /// </summary>
        Info,

        /// <summary>
        /// 警告消息
        /// </summary>
        Warn,

        /// <summary>
        /// 错误消息
        /// </summary>
        Error,

        /// <summary>
        /// 致命消息
        /// </summary>
        Fatal,


    }



    /// <summary>
    /// 多少位计算模式 x86 / x64
    /// </summary>
    public enum BitOperatingSystemTypeEnum
    {
        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,
        /// <summary>
        /// 32位
        /// </summary>
        x86,
        /// <summary>
        /// 64位
        /// </summary>
        x64,
        /// <summary>
        /// 未知
        /// </summary>
        UnKnow,
    }



    /// <summary>
    /// 佳运通 日志 类型
    /// </summary>
    public enum LoggerTypeEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        UnDefine,

        /// <summary>
        /// 文件日志
        /// </summary>
        FileLogger,

        /// <summary>
        /// 数据库日志
        /// </summary>
        DataBaseLogger,

    }


    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DbTypeEnum
    {

        UnDefine,

        SqlServer,

        MySql,

        //Oracle,

    }



    /// <summary>
    /// IOC 注入 key 值 枚举
    /// </summary>
    public enum IocMapperKeyEnum
    {

        /// <summary>
        /// 默认key值
        /// </summary>
        DefaultKey,
        /// <summary>
        /// DapperExtensions  MappingAssemblies  映射程序集  列表 注入 表示KEY
        /// </summary>
        DapperExtensionsMappingAssembliesKey,

    }


    /// <summary>
    /// 密钥长度(加密级别) 1024/2048/3072/7680/15360
    /// </summary>
    public enum RsaKeySizeTypeEnum
    {
        V1024 = 1024,
        V2048 = 2048,
        V3072 = 3072,
        V7680 = 7680,
        V15360 = 15360,
    }


    #region HTTP 相关枚举



    /// <summary>
    /// Enum HttpContentTypeEnum
    /// </summary>
    public enum HttpContentTypeEnum
    {


        /// <summary>
        /// 未定义
        /// </summary>
        [TitleEnum("未定义")]
        UnDefine,

        /// <summary>
        /// 未知类型
        /// </summary>
        [TitleEnum("未知类型")]
        UnKnown,


        //没使用大小写名称 原因是 application/vnd.android.package-archive 这种数据 用 '_' 形式命名规则 更直观 


        /// <summary>
        /// application/fractals
        /// </summary>
        [TitleEnum("application/fractals")]
        application_fractals,


        /// <summary>
        /// application/futuresplash
        /// </summary>
        [TitleEnum("application/futuresplash")]
        application_futuresplash,


        /// <summary>
        /// application/hta
        /// </summary>
        [TitleEnum("application/hta")]
        application_hta,


        /// <summary>
        /// application/json
        /// </summary>
        [TitleEnum("application/json")]
        application_json,


        /// <summary>
        /// application/mac-binhex40
        /// </summary>
        [TitleEnum("application/mac-binhex40")]
        application_mac_binhex40,


        /// <summary>
        /// application/msaccess
        /// </summary>
        [TitleEnum("application/msaccess")]
        application_msaccess,


        /// <summary>
        /// application/msword
        /// </summary>
        [TitleEnum("application/msword")]
        application_msword,


        /// <summary>
        /// application/octet-stream
        /// </summary>
        [TitleEnum("application/octet-stream")]
        application_octet_stream,


        /// <summary>
        /// application/pdf
        /// </summary>
        [TitleEnum("application/pdf")]
        application_pdf,


        /// <summary>
        /// application/pics-rules
        /// </summary>
        [TitleEnum("application/pics-rules")]
        application_pics_rules,


        /// <summary>
        /// application/pkcs10
        /// </summary>
        [TitleEnum("application/pkcs10")]
        application_pkcs10,


        /// <summary>
        /// application/pkcs7-mime
        /// </summary>
        [TitleEnum("application/pkcs7-mime")]
        application_pkcs7_mime,


        /// <summary>
        /// application/pkcs7-signature
        /// </summary>
        [TitleEnum("application/pkcs7-signature")]
        application_pkcs7_signature,


        /// <summary>
        /// application/pkix-crl
        /// </summary>
        [TitleEnum("application/pkix-crl")]
        application_pkix_crl,


        /// <summary>
        /// application/postscript
        /// </summary>
        [TitleEnum("application/postscript")]
        application_postscript,


        /// <summary>
        /// application/rat-file
        /// </summary>
        [TitleEnum("application/rat-file")]
        application_rat_file,


        /// <summary>
        /// application/sdp
        /// </summary>
        [TitleEnum("application/sdp")]
        application_sdp,


        /// <summary>
        /// application/smil
        /// </summary>
        [TitleEnum("application/smil")]
        application_smil,


        /// <summary>
        /// application/streamingmedia
        /// </summary>
        [TitleEnum("application/streamingmedia")]
        application_streamingmedia,


        /// <summary>
        /// application/vnd.adobe.edn
        /// </summary>
        [TitleEnum("application/vnd.adobe.edn")]
        application_vnd_adobe_edn,


        /// <summary>
        /// application/vnd.adobe.pdx
        /// </summary>
        [TitleEnum("application/vnd.adobe.pdx")]
        application_vnd_adobe_pdx,


        /// <summary>
        /// application/vnd.adobe.rmf
        /// </summary>
        [TitleEnum("application/vnd.adobe.rmf")]
        application_vnd_adobe_rmf,


        /// <summary>
        /// application/vnd.adobe.workflow
        /// </summary>
        [TitleEnum("application/vnd.adobe.workflow")]
        application_vnd_adobe_workflow,


        /// <summary>
        /// application/vnd.adobe.xdp
        /// </summary>
        [TitleEnum("application/vnd.adobe.xdp")]
        application_vnd_adobe_xdp,


        /// <summary>
        /// application/vnd.adobe.xfd
        /// </summary>
        [TitleEnum("application/vnd.adobe.xfd")]
        application_vnd_adobe_xfd,


        /// <summary>
        /// application/vnd.adobe.xfdf
        /// </summary>
        [TitleEnum("application/vnd.adobe.xfdf")]
        application_vnd_adobe_xfdf,


        /// <summary>
        /// application/vnd.android.package-archive
        /// </summary>
        [TitleEnum("application/vnd.android.package-archive")]
        application_vnd_android_package_archive,


        /// <summary>
        /// application/vnd.fdf
        /// </summary>
        [TitleEnum("application/vnd.fdf")]
        application_vnd_fdf,


        /// <summary>
        /// application/vnd.iphone
        /// </summary>
        [TitleEnum("application/vnd.iphone")]
        application_vnd_iphone,


        /// <summary>
        /// application/vnd.ms-excel
        /// </summary>
        [TitleEnum("application/vnd.ms-excel")]
        application_vnd_ms_excel,


        /// <summary>
        /// application/vnd.ms-pki.certstore
        /// </summary>
        [TitleEnum("application/vnd.ms-pki.certstore")]
        application_vnd_ms_pki_certstore,


        /// <summary>
        /// application/vnd.ms-pki.pko
        /// </summary>
        [TitleEnum("application/vnd.ms-pki.pko")]
        application_vnd_ms_pki_pko,


        /// <summary>
        /// application/vnd.ms-pki.seccat
        /// </summary>
        [TitleEnum("application/vnd.ms-pki.seccat")]
        application_vnd_ms_pki_seccat,


        /// <summary>
        /// application/vnd.ms-pki.stl
        /// </summary>
        [TitleEnum("application/vnd.ms-pki.stl")]
        application_vnd_ms_pki_stl,


        /// <summary>
        /// application/vnd.ms-powerpoint
        /// </summary>
        [TitleEnum("application/vnd.ms-powerpoint")]
        application_vnd_ms_powerpoint,


        /// <summary>
        /// application/vnd.ms-project
        /// </summary>
        [TitleEnum("application/vnd.ms-project")]
        application_vnd_ms_project,


        /// <summary>
        /// application/vnd.ms-wpl
        /// </summary>
        [TitleEnum("application/vnd.ms-wpl")]
        application_vnd_ms_wpl,


        /// <summary>
        /// application/vnd.rn-realmedia
        /// </summary>
        [TitleEnum("application/vnd.rn-realmedia")]
        application_vnd_rn_realmedia,


        /// <summary>
        /// application/vnd.rn-realmedia-secure
        /// </summary>
        [TitleEnum("application/vnd.rn-realmedia-secure")]
        application_vnd_rn_realmedia_secure,


        /// <summary>
        /// application/vnd.rn-realmedia-vbr
        /// </summary>
        [TitleEnum("application/vnd.rn-realmedia-vbr")]
        application_vnd_rn_realmedia_vbr,


        /// <summary>
        /// application/vnd.rn-realplayer
        /// </summary>
        [TitleEnum("application/vnd.rn-realplayer")]
        application_vnd_rn_realplayer,


        /// <summary>
        /// application/vnd.rn-realsystem-rjs
        /// </summary>
        [TitleEnum("application/vnd.rn-realsystem-rjs")]
        application_vnd_rn_realsystem_rjs,


        /// <summary>
        /// application/vnd.rn-realsystem-rjt
        /// </summary>
        [TitleEnum("application/vnd.rn-realsystem-rjt")]
        application_vnd_rn_realsystem_rjt,


        /// <summary>
        /// application/vnd.rn-realsystem-rmj
        /// </summary>
        [TitleEnum("application/vnd.rn-realsystem-rmj")]
        application_vnd_rn_realsystem_rmj,


        /// <summary>
        /// application/vnd.rn-realsystem-rmx
        /// </summary>
        [TitleEnum("application/vnd.rn-realsystem-rmx")]
        application_vnd_rn_realsystem_rmx,


        /// <summary>
        /// application/vnd.rn-recording
        /// </summary>
        [TitleEnum("application/vnd.rn-recording")]
        application_vnd_rn_recording,


        /// <summary>
        /// application/vnd.rn-rn_music_package
        /// </summary>
        [TitleEnum("application/vnd.rn-rn_music_package")]
        application_vnd_rn_rn_music_package,


        /// <summary>
        /// application/vnd.rn-rsml
        /// </summary>
        [TitleEnum("application/vnd.rn-rsml")]
        application_vnd_rn_rsml,


        /// <summary>
        /// application/vnd.symbian.install
        /// </summary>
        [TitleEnum("application/vnd.symbian.install")]
        application_vnd_symbian_install,


        /// <summary>
        /// application/vnd.visio
        /// </summary>
        [TitleEnum("application/vnd.visio")]
        application_vnd_visio,


        /// <summary>
        /// application/x-001
        /// </summary>
        [TitleEnum("application/x-001")]
        application_x_001,


        /// <summary>
        /// application/x-301
        /// </summary>
        [TitleEnum("application/x-301")]
        application_x_301,


        /// <summary>
        /// application/x-906
        /// </summary>
        [TitleEnum("application/x-906")]
        application_x_906,


        /// <summary>
        /// application/x-a11
        /// </summary>
        [TitleEnum("application/x-a11")]
        application_x_a11,


        /// <summary>
        /// application/x-anv
        /// </summary>
        [TitleEnum("application/x-anv")]
        application_x_anv,


        /// <summary>
        /// application/x-bittorrent
        /// </summary>
        [TitleEnum("application/x-bittorrent")]
        application_x_bittorrent,


        /// <summary>
        /// application/x-bmp
        /// </summary>
        [TitleEnum("application/x-bmp")]
        application_x_bmp,


        /// <summary>
        /// application/x-bot
        /// </summary>
        [TitleEnum("application/x-bot")]
        application_x_bot,


        /// <summary>
        /// application/x-c4t
        /// </summary>
        [TitleEnum("application/x-c4t")]
        application_x_c4t,


        /// <summary>
        /// application/x-c90
        /// </summary>
        [TitleEnum("application/x-c90")]
        application_x_c90,


        /// <summary>
        /// application/x-cals
        /// </summary>
        [TitleEnum("application/x-cals")]
        application_x_cals,


        /// <summary>
        /// application/x-cdr
        /// </summary>
        [TitleEnum("application/x-cdr")]
        application_x_cdr,


        /// <summary>
        /// application/x-cel
        /// </summary>
        [TitleEnum("application/x-cel")]
        application_x_cel,


        /// <summary>
        /// application/x-cgm
        /// </summary>
        [TitleEnum("application/x-cgm")]
        application_x_cgm,


        /// <summary>
        /// application/x-cit
        /// </summary>
        [TitleEnum("application/x-cit")]
        application_x_cit,


        /// <summary>
        /// application/x-cmp
        /// </summary>
        [TitleEnum("application/x-cmp")]
        application_x_cmp,


        /// <summary>
        /// application/x-cmx
        /// </summary>
        [TitleEnum("application/x-cmx")]
        application_x_cmx,


        /// <summary>
        /// application/x-cot
        /// </summary>
        [TitleEnum("application/x-cot")]
        application_x_cot,


        /// <summary>
        /// application/x-csi
        /// </summary>
        [TitleEnum("application/x-csi")]
        application_x_csi,


        /// <summary>
        /// application/x-cut
        /// </summary>
        [TitleEnum("application/x-cut")]
        application_x_cut,


        /// <summary>
        /// application/x-dbf
        /// </summary>
        [TitleEnum("application/x-dbf")]
        application_x_dbf,


        /// <summary>
        /// application/x-dbm
        /// </summary>
        [TitleEnum("application/x-dbm")]
        application_x_dbm,


        /// <summary>
        /// application/x-dbx
        /// </summary>
        [TitleEnum("application/x-dbx")]
        application_x_dbx,


        /// <summary>
        /// application/x-dcx
        /// </summary>
        [TitleEnum("application/x-dcx")]
        application_x_dcx,


        /// <summary>
        /// application/x-dgn
        /// </summary>
        [TitleEnum("application/x-dgn")]
        application_x_dgn,


        /// <summary>
        /// application/x-dib
        /// </summary>
        [TitleEnum("application/x-dib")]
        application_x_dib,


        /// <summary>
        /// application/x-drw
        /// </summary>
        [TitleEnum("application/x-drw")]
        application_x_drw,


        /// <summary>
        /// application/x-dwf
        /// </summary>
        [TitleEnum("application/x-dwf")]
        application_x_dwf,


        /// <summary>
        /// application/x-dwg
        /// </summary>
        [TitleEnum("application/x-dwg")]
        application_x_dwg,


        /// <summary>
        /// application/x-dxb
        /// </summary>
        [TitleEnum("application/x-dxb")]
        application_x_dxb,


        /// <summary>
        /// application/x-dxf
        /// </summary>
        [TitleEnum("application/x-dxf")]
        application_x_dxf,


        /// <summary>
        /// application/x-ebx
        /// </summary>
        [TitleEnum("application/x-ebx")]
        application_x_ebx,


        /// <summary>
        /// application/x-emf
        /// </summary>
        [TitleEnum("application/x-emf")]
        application_x_emf,


        /// <summary>
        /// application/x-epi
        /// </summary>
        [TitleEnum("application/x-epi")]
        application_x_epi,


        /// <summary>
        /// application/x-frm
        /// </summary>
        [TitleEnum("application/x-frm")]
        application_x_frm,


        /// <summary>
        /// application/x-g4
        /// </summary>
        [TitleEnum("application/x-g4")]
        application_x_g4,


        /// <summary>
        /// application/x-gbr
        /// </summary>
        [TitleEnum("application/x-gbr")]
        application_x_gbr,


        /// <summary>
        /// application/x-gl2
        /// </summary>
        [TitleEnum("application/x-gl2")]
        application_x_gl2,


        /// <summary>
        /// application/x-gp4
        /// </summary>
        [TitleEnum("application/x-gp4")]
        application_x_gp4,


        /// <summary>
        /// application/x-hgl
        /// </summary>
        [TitleEnum("application/x-hgl")]
        application_x_hgl,


        /// <summary>
        /// application/x-hmr
        /// </summary>
        [TitleEnum("application/x-hmr")]
        application_x_hmr,


        /// <summary>
        /// application/x-hpgl
        /// </summary>
        [TitleEnum("application/x-hpgl")]
        application_x_hpgl,


        /// <summary>
        /// application/x-hpl
        /// </summary>
        [TitleEnum("application/x-hpl")]
        application_x_hpl,


        /// <summary>
        /// application/x-hrf
        /// </summary>
        [TitleEnum("application/x-hrf")]
        application_x_hrf,


        /// <summary>
        /// application/x-icb
        /// </summary>
        [TitleEnum("application/x-icb")]
        application_x_icb,


        /// <summary>
        /// application/x-ico
        /// </summary>
        [TitleEnum("application/x-ico")]
        application_x_ico,


        /// <summary>
        /// application/x-icq
        /// </summary>
        [TitleEnum("application/x-icq")]
        application_x_icq,


        /// <summary>
        /// application/x-iff
        /// </summary>
        [TitleEnum("application/x-iff")]
        application_x_iff,


        /// <summary>
        /// application/x-igs
        /// </summary>
        [TitleEnum("application/x-igs")]
        application_x_igs,


        /// <summary>
        /// application/x-img
        /// </summary>
        [TitleEnum("application/x-img")]
        application_x_img,


        /// <summary>
        /// application/x-internet-signup
        /// </summary>
        [TitleEnum("application/x-internet-signup")]
        application_x_internet_signup,


        /// <summary>
        /// application/x-iphone
        /// </summary>
        [TitleEnum("application/x-iphone")]
        application_x_iphone,


        /// <summary>
        /// application/x-javascript
        /// </summary>
        [TitleEnum("application/x-javascript")]
        application_x_javascript,


        /// <summary>
        /// application/x-jpe
        /// </summary>
        [TitleEnum("application/x-jpe")]
        application_x_jpe,


        /// <summary>
        /// application/x-jpg
        /// </summary>
        [TitleEnum("application/x-jpg")]
        application_x_jpg,


        /// <summary>
        /// application/x-laplayer-reg
        /// </summary>
        [TitleEnum("application/x-laplayer-reg")]
        application_x_laplayer_reg,


        /// <summary>
        /// application/x-latex
        /// </summary>
        [TitleEnum("application/x-latex")]
        application_x_latex,


        /// <summary>
        /// application/x-lbm
        /// </summary>
        [TitleEnum("application/x-lbm")]
        application_x_lbm,


        /// <summary>
        /// application/x-ltr
        /// </summary>
        [TitleEnum("application/x-ltr")]
        application_x_ltr,


        /// <summary>
        /// application/x-mac
        /// </summary>
        [TitleEnum("application/x-mac")]
        application_x_mac,


        /// <summary>
        /// application/x-mdb
        /// </summary>
        [TitleEnum("application/x-mdb")]
        application_x_mdb,


        /// <summary>
        /// application/x-mi
        /// </summary>
        [TitleEnum("application/x-mi")]
        application_x_mi,


        /// <summary>
        /// application/x-mil
        /// </summary>
        [TitleEnum("application/x-mil")]
        application_x_mil,


        /// <summary>
        /// application/x-mmxp
        /// </summary>
        [TitleEnum("application/x-mmxp")]
        application_x_mmxp,


        /// <summary>
        /// application/x-ms-wmd
        /// </summary>
        [TitleEnum("application/x-ms-wmd")]
        application_x_ms_wmd,


        /// <summary>
        /// application/x-ms-wmz
        /// </summary>
        [TitleEnum("application/x-ms-wmz")]
        application_x_ms_wmz,


        /// <summary>
        /// application/x-msdownload
        /// </summary>
        [TitleEnum("application/x-msdownload")]
        application_x_msdownload,


        /// <summary>
        /// application/x-netcdf
        /// </summary>
        [TitleEnum("application/x-netcdf")]
        application_x_netcdf,


        /// <summary>
        /// application/x-nrf
        /// </summary>
        [TitleEnum("application/x-nrf")]
        application_x_nrf,


        /// <summary>
        /// application/x-out
        /// </summary>
        [TitleEnum("application/x-out")]
        application_x_out,


        /// <summary>
        /// application/x-pc5
        /// </summary>
        [TitleEnum("application/x-pc5")]
        application_x_pc5,


        /// <summary>
        /// application/x-pci
        /// </summary>
        [TitleEnum("application/x-pci")]
        application_x_pci,


        /// <summary>
        /// application/x-pcl
        /// </summary>
        [TitleEnum("application/x-pcl")]
        application_x_pcl,


        /// <summary>
        /// application/x-pcx
        /// </summary>
        [TitleEnum("application/x-pcx")]
        application_x_pcx,


        /// <summary>
        /// application/x-perl
        /// </summary>
        [TitleEnum("application/x-perl")]
        application_x_perl,


        /// <summary>
        /// application/x-pgl
        /// </summary>
        [TitleEnum("application/x-pgl")]
        application_x_pgl,


        /// <summary>
        /// application/x-pic
        /// </summary>
        [TitleEnum("application/x-pic")]
        application_x_pic,


        /// <summary>
        /// application/x-pkcs12
        /// </summary>
        [TitleEnum("application/x-pkcs12")]
        application_x_pkcs12,


        /// <summary>
        /// application/x-pkcs7-certificates
        /// </summary>
        [TitleEnum("application/x-pkcs7-certificates")]
        application_x_pkcs7_certificates,


        /// <summary>
        /// application/x-pkcs7-certreqresp
        /// </summary>
        [TitleEnum("application/x-pkcs7-certreqresp")]
        application_x_pkcs7_certreqresp,


        /// <summary>
        /// application/x-plt
        /// </summary>
        [TitleEnum("application/x-plt")]
        application_x_plt,


        /// <summary>
        /// application/x-png
        /// </summary>
        [TitleEnum("application/x-png")]
        application_x_png,


        /// <summary>
        /// application/x-ppm
        /// </summary>
        [TitleEnum("application/x-ppm")]
        application_x_ppm,


        /// <summary>
        /// application/x-ppt
        /// </summary>
        [TitleEnum("application/x-ppt")]
        application_x_ppt,


        /// <summary>
        /// application/x-pr
        /// </summary>
        [TitleEnum("application/x-pr")]
        application_x_pr,


        /// <summary>
        /// application/x-prn
        /// </summary>
        [TitleEnum("application/x-prn")]
        application_x_prn,


        /// <summary>
        /// application/x-prt
        /// </summary>
        [TitleEnum("application/x-prt")]
        application_x_prt,


        /// <summary>
        /// application/x-ps
        /// </summary>
        [TitleEnum("application/x-ps")]
        application_x_ps,


        /// <summary>
        /// application/x-ptn
        /// </summary>
        [TitleEnum("application/x-ptn")]
        application_x_ptn,


        /// <summary>
        /// application/x-ras
        /// </summary>
        [TitleEnum("application/x-ras")]
        application_x_ras,


        /// <summary>
        /// application/x-red
        /// </summary>
        [TitleEnum("application/x-red")]
        application_x_red,


        /// <summary>
        /// application/x-rgb
        /// </summary>
        [TitleEnum("application/x-rgb")]
        application_x_rgb,


        /// <summary>
        /// application/x-rlc
        /// </summary>
        [TitleEnum("application/x-rlc")]
        application_x_rlc,


        /// <summary>
        /// application/x-rle
        /// </summary>
        [TitleEnum("application/x-rle")]
        application_x_rle,


        /// <summary>
        /// application/x-rtf
        /// </summary>
        [TitleEnum("application/x-rtf")]
        application_x_rtf,


        /// <summary>
        /// application/x-sam
        /// </summary>
        [TitleEnum("application/x-sam")]
        application_x_sam,


        /// <summary>
        /// application/x-sat
        /// </summary>
        [TitleEnum("application/x-sat")]
        application_x_sat,


        /// <summary>
        /// application/x-sdw
        /// </summary>
        [TitleEnum("application/x-sdw")]
        application_x_sdw,


        /// <summary>
        /// application/x-shockwave-flash
        /// </summary>
        [TitleEnum("application/x-shockwave-flash")]
        application_x_shockwave_flash,


        /// <summary>
        /// application/x-silverlight-app
        /// </summary>
        [TitleEnum("application/x-silverlight-app")]
        application_x_silverlight_app,


        /// <summary>
        /// application/x-slb
        /// </summary>
        [TitleEnum("application/x-slb")]
        application_x_slb,


        /// <summary>
        /// application/x-sld
        /// </summary>
        [TitleEnum("application/x-sld")]
        application_x_sld,


        /// <summary>
        /// application/x-smk
        /// </summary>
        [TitleEnum("application/x-smk")]
        application_x_smk,


        /// <summary>
        /// application/x-stuffit
        /// </summary>
        [TitleEnum("application/x-stuffit")]
        application_x_stuffit,


        /// <summary>
        /// application/x-sty
        /// </summary>
        [TitleEnum("application/x-sty")]
        application_x_sty,


        /// <summary>
        /// application/x-tdf
        /// </summary>
        [TitleEnum("application/x-tdf")]
        application_x_tdf,


        /// <summary>
        /// application/x-tg4
        /// </summary>
        [TitleEnum("application/x-tg4")]
        application_x_tg4,


        /// <summary>
        /// application/x-tga
        /// </summary>
        [TitleEnum("application/x-tga")]
        application_x_tga,


        /// <summary>
        /// application/x-tif
        /// </summary>
        [TitleEnum("application/x-tif")]
        application_x_tif,


        /// <summary>
        /// application/x-troff-man
        /// </summary>
        [TitleEnum("application/x-troff-man")]
        application_x_troff_man,


        /// <summary>
        /// application/x-vda
        /// </summary>
        [TitleEnum("application/x-vda")]
        application_x_vda,


        /// <summary>
        /// application/x-vpeg005
        /// </summary>
        [TitleEnum("application/x-vpeg005")]
        application_x_vpeg005,


        /// <summary>
        /// application/x-vsd
        /// </summary>
        [TitleEnum("application/x-vsd")]
        application_x_vsd,


        /// <summary>
        /// application/x-vst
        /// </summary>
        [TitleEnum("application/x-vst")]
        application_x_vst,


        /// <summary>
        /// application/x-wb1
        /// </summary>
        [TitleEnum("application/x-wb1")]
        application_x_wb1,


        /// <summary>
        /// application/x-wb2
        /// </summary>
        [TitleEnum("application/x-wb2")]
        application_x_wb2,


        /// <summary>
        /// application/x-wb3
        /// </summary>
        [TitleEnum("application/x-wb3")]
        application_x_wb3,


        /// <summary>
        /// application/x-wk3
        /// </summary>
        [TitleEnum("application/x-wk3")]
        application_x_wk3,


        /// <summary>
        /// application/x-wk4
        /// </summary>
        [TitleEnum("application/x-wk4")]
        application_x_wk4,


        /// <summary>
        /// application/x-wkq
        /// </summary>
        [TitleEnum("application/x-wkq")]
        application_x_wkq,


        /// <summary>
        /// application/x-wks
        /// </summary>
        [TitleEnum("application/x-wks")]
        application_x_wks,


        /// <summary>
        /// application/x-wmf
        /// </summary>
        [TitleEnum("application/x-wmf")]
        application_x_wmf,


        /// <summary>
        /// application/x-wp6
        /// </summary>
        [TitleEnum("application/x-wp6")]
        application_x_wp6,


        /// <summary>
        /// application/x-wpd
        /// </summary>
        [TitleEnum("application/x-wpd")]
        application_x_wpd,


        /// <summary>
        /// application/x-wpg
        /// </summary>
        [TitleEnum("application/x-wpg")]
        application_x_wpg,


        /// <summary>
        /// application/x-wq1
        /// </summary>
        [TitleEnum("application/x-wq1")]
        application_x_wq1,


        /// <summary>
        /// application/x-wr1
        /// </summary>
        [TitleEnum("application/x-wr1")]
        application_x_wr1,


        /// <summary>
        /// application/x-wri
        /// </summary>
        [TitleEnum("application/x-wri")]
        application_x_wri,


        /// <summary>
        /// application/x-wrk
        /// </summary>
        [TitleEnum("application/x-wrk")]
        application_x_wrk,


        /// <summary>
        /// application/x-ws
        /// </summary>
        [TitleEnum("application/x-ws")]
        application_x_ws,


        /// <summary>
        /// application/x-x_b
        /// </summary>
        [TitleEnum("application/x-x_b")]
        application_x_x_b,


        /// <summary>
        /// application/x-x_t
        /// </summary>
        [TitleEnum("application/x-x_t")]
        application_x_x_t,


        /// <summary>
        /// application/x-x509-ca-cert
        /// </summary>
        [TitleEnum("application/x-x509-ca-cert")]
        application_x_x509_ca_cert,


        /// <summary>
        /// application/x-xls
        /// </summary>
        [TitleEnum("application/x-xls")]
        application_x_xls,


        /// <summary>
        /// application/x-xlw
        /// </summary>
        [TitleEnum("application/x-xlw")]
        application_x_xlw,


        /// <summary>
        /// application/x-xwd
        /// </summary>
        [TitleEnum("application/x-xwd")]
        application_x_xwd,


        /// <summary>
        /// application/xml
        /// </summary>
        [TitleEnum("application/xml")]
        application_xml,


        /// <summary>
        /// audio/aiff
        /// </summary>
        [TitleEnum("audio/aiff")]
        audio_aiff,


        /// <summary>
        /// audio/basic
        /// </summary>
        [TitleEnum("audio/basic")]
        audio_basic,


        /// <summary>
        /// audio/mid
        /// </summary>
        [TitleEnum("audio/mid")]
        audio_mid,


        /// <summary>
        /// audio/mp1
        /// </summary>
        [TitleEnum("audio/mp1")]
        audio_mp1,


        /// <summary>
        /// audio/mp2
        /// </summary>
        [TitleEnum("audio/mp2")]
        audio_mp2,


        /// <summary>
        /// audio/mp3
        /// </summary>
        [TitleEnum("audio/mp3")]
        audio_mp3,


        /// <summary>
        /// audio/mpegurl
        /// </summary>
        [TitleEnum("audio/mpegurl")]
        audio_mpegurl,


        /// <summary>
        /// audio/rn-mpeg
        /// </summary>
        [TitleEnum("audio/rn-mpeg")]
        audio_rn_mpeg,


        /// <summary>
        /// audio/scpls
        /// </summary>
        [TitleEnum("audio/scpls")]
        audio_scpls,


        /// <summary>
        /// audio/vnd.rn-realaudio
        /// </summary>
        [TitleEnum("audio/vnd.rn-realaudio")]
        audio_vnd_rn_realaudio,


        /// <summary>
        /// audio/wav
        /// </summary>
        [TitleEnum("audio/wav")]
        audio_wav,


        /// <summary>
        /// audio/x-la-lms
        /// </summary>
        [TitleEnum("audio/x-la-lms")]
        audio_x_la_lms,


        /// <summary>
        /// audio/x-liquid-file
        /// </summary>
        [TitleEnum("audio/x-liquid-file")]
        audio_x_liquid_file,


        /// <summary>
        /// audio/x-liquid-secure
        /// </summary>
        [TitleEnum("audio/x-liquid-secure")]
        audio_x_liquid_secure,


        /// <summary>
        /// audio/x-mei-aac
        /// </summary>
        [TitleEnum("audio/x-mei-aac")]
        audio_x_mei_aac,


        /// <summary>
        /// audio/x-ms-wax
        /// </summary>
        [TitleEnum("audio/x-ms-wax")]
        audio_x_ms_wax,


        /// <summary>
        /// audio/x-ms-wma
        /// </summary>
        [TitleEnum("audio/x-ms-wma")]
        audio_x_ms_wma,


        /// <summary>
        /// audio/x-musicnet-download
        /// </summary>
        [TitleEnum("audio/x-musicnet-download")]
        audio_x_musicnet_download,


        /// <summary>
        /// audio/x-musicnet-stream
        /// </summary>
        [TitleEnum("audio/x-musicnet-stream")]
        audio_x_musicnet_stream,


        /// <summary>
        /// audio/x-pn-realaudio
        /// </summary>
        [TitleEnum("audio/x-pn-realaudio")]
        audio_x_pn_realaudio,


        /// <summary>
        /// audio/x-pn-realaudio-plugin
        /// </summary>
        [TitleEnum("audio/x-pn-realaudio-plugin")]
        audio_x_pn_realaudio_plugin,


        /// <summary>
        /// drawing/907
        /// </summary>
        [TitleEnum("drawing/907")]
        drawing_907,


        /// <summary>
        /// drawing/x-slk
        /// </summary>
        [TitleEnum("drawing/x-slk")]
        drawing_x_slk,


        /// <summary>
        /// drawing/x-top
        /// </summary>
        [TitleEnum("drawing/x-top")]
        drawing_x_top,


        /// <summary>
        /// image/fax
        /// </summary>
        [TitleEnum("image/fax")]
        image_fax,


        /// <summary>
        /// image/gif
        /// </summary>
        [TitleEnum("image/gif")]
        image_gif,


        /// <summary>
        /// image/jpeg
        /// </summary>
        [TitleEnum("image/jpeg")]
        image_jpeg,


        /// <summary>
        /// image/pnetvue
        /// </summary>
        [TitleEnum("image/pnetvue")]
        image_pnetvue,


        /// <summary>
        /// image/png
        /// </summary>
        [TitleEnum("image/png")]
        image_png,


        /// <summary>
        /// image/tiff
        /// </summary>
        [TitleEnum("image/tiff")]
        image_tiff,


        /// <summary>
        /// image/vnd.rn-realpix
        /// </summary>
        [TitleEnum("image/vnd.rn-realpix")]
        image_vnd_rn_realpix,


        /// <summary>
        /// image/vnd.wap.wbmp
        /// </summary>
        [TitleEnum("image/vnd.wap.wbmp")]
        image_vnd_wap_wbmp,


        /// <summary>
        /// image/x-icon
        /// </summary>
        [TitleEnum("image/x-icon")]
        image_x_icon,


        /// <summary>
        /// java/*
        /// </summary>
        [TitleEnum("java/*")]
        java_common,


        /// <summary>
        /// message/rfc822
        /// </summary>
        [TitleEnum("message/rfc822")]
        message_rfc822,


        /// <summary>
        /// model/vnd.dwf
        /// </summary>
        [TitleEnum("model/vnd.dwf")]
        model_vnd_dwf,


        /// <summary>
        /// multipart/form-data
        /// </summary>
        [TitleEnum("multipart/form-data")]
        multipart_form_data,


        /// <summary>
        /// text/asa
        /// </summary>
        [TitleEnum("text/asa")]
        text_asa,


        /// <summary>
        /// text/asp
        /// </summary>
        [TitleEnum("text/asp")]
        text_asp,


        /// <summary>
        /// text/css
        /// </summary>
        [TitleEnum("text/css")]
        text_css,


        /// <summary>
        /// text/h323
        /// </summary>
        [TitleEnum("text/h323")]
        text_h323,


        /// <summary>
        /// text/html
        /// </summary>
        [TitleEnum("text/html")]
        text_html,


        /// <summary>
        /// text/iuls
        /// </summary>
        [TitleEnum("text/iuls")]
        text_iuls,


        /// <summary>
        /// text/plain
        /// </summary>
        [TitleEnum("text/plain")]
        text_plain,


        /// <summary>
        /// text/scriptlet
        /// </summary>
        [TitleEnum("text/scriptlet")]
        text_scriptlet,


        /// <summary>
        /// text/vnd.rn-realtext
        /// </summary>
        [TitleEnum("text/vnd.rn-realtext")]
        text_vnd_rn_realtext,


        /// <summary>
        /// text/vnd.rn-realtext3d
        /// </summary>
        [TitleEnum("text/vnd.rn-realtext3d")]
        text_vnd_rn_realtext3d,


        /// <summary>
        /// text/vnd.wap.wml
        /// </summary>
        [TitleEnum("text/vnd.wap.wml")]
        text_vnd_wap_wml,


        /// <summary>
        /// text/webviewhtml
        /// </summary>
        [TitleEnum("text/webviewhtml")]
        text_webviewhtml,


        /// <summary>
        /// text/x-component
        /// </summary>
        [TitleEnum("text/x-component")]
        text_x_component,


        /// <summary>
        /// text/x-ms-odc
        /// </summary>
        [TitleEnum("text/x-ms-odc")]
        text_x_ms_odc,


        /// <summary>
        /// text/x-vcard
        /// </summary>
        [TitleEnum("text/x-vcard")]
        text_x_vcard,


        /// <summary>
        /// text/xml
        /// </summary>
        [TitleEnum("text/xml")]
        text_xml,


        /// <summary>
        /// video/avi
        /// </summary>
        [TitleEnum("video/avi")]
        video_avi,


        /// <summary>
        /// video/mpeg
        /// </summary>
        [TitleEnum("video/mpeg")]
        video_mpeg,


        /// <summary>
        /// video/mpeg4
        /// </summary>
        [TitleEnum("video/mpeg4")]
        video_mpeg4,


        /// <summary>
        /// video/mpg
        /// </summary>
        [TitleEnum("video/mpg")]
        video_mpg,


        /// <summary>
        /// video/vnd.rn-realvideo
        /// </summary>
        [TitleEnum("video/vnd.rn-realvideo")]
        video_vnd_rn_realvideo,


        /// <summary>
        /// video/x-ivf
        /// </summary>
        [TitleEnum("video/x-ivf")]
        video_x_ivf,


        /// <summary>
        /// video/x-mpeg
        /// </summary>
        [TitleEnum("video/x-mpeg")]
        video_x_mpeg,


        /// <summary>
        /// video/x-mpg
        /// </summary>
        [TitleEnum("video/x-mpg")]
        video_x_mpg,


        /// <summary>
        /// video/x-ms-asf
        /// </summary>
        [TitleEnum("video/x-ms-asf")]
        video_x_ms_asf,


        /// <summary>
        /// video/x-ms-wm
        /// </summary>
        [TitleEnum("video/x-ms-wm")]
        video_x_ms_wm,


        /// <summary>
        /// video/x-ms-wmv
        /// </summary>
        [TitleEnum("video/x-ms-wmv")]
        video_x_ms_wmv,


        /// <summary>
        /// video/x-ms-wmx
        /// </summary>
        [TitleEnum("video/x-ms-wmx")]
        video_x_ms_wmx,


        /// <summary>
        /// video/x-ms-wvx
        /// </summary>
        [TitleEnum("video/x-ms-wvx")]
        video_x_ms_wvx,


        /// <summary>
        /// video/x-sgi-movie
        /// </summary>
        [TitleEnum("video/x-sgi-movie")]
        video_x_sgi_movie,


    }



    /// <summary>
    /// Owin 环境变量 字典 主键枚举值
    /// </summary>
    public enum OwinContextEnvironmentKeyEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        [TitleEnum("未定义")]
        UnDefine,

        /// <summary>
        /// 未知类型
        /// </summary>
        [TitleEnum("未知类型")]
        UnKnown,

        /// <summary>
        /// host.AppMode
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.HOST_APP_MODE)]
        HostAppMode,
        /// <summary>
        /// host.AppName
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.HOST_APP_NAME)]
        HostAppName,

        /// <summary>
        /// host.OnAppDisposing
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.HOST_ON_APP_DISPOSING)]
        HostOnAppDisposing,

        /// <summary>
        /// host.TraceOutput
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.HOST_TRACE_OUTPUT)]
        HostTraceOutput,

        /// <summary>
        /// integratedpipeline.Context
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.INTEGRATED_PIPELINE_CONTEXT)]
        IntegratedpipelineContext,

        /// <summary>
        /// integratedpipeline.CurrentStage
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.INTEGRATED_PIPELINE_CURRENT_STAGE)]
        IntegratedpipelineCurrentStage,

        /// <summary>
        /// owin.CallCancelled
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_CALL_CANCELLED)]
        OwinCallCancelled,

        /// <summary>
        /// owin.RequestBody
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_BODY)]
        OwinRequestBody,

        /// <summary>
        /// owin.RequestHeaders
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_HEADERS)]
        OwinRequestHeaders,

        /// <summary>
        /// owin.RequestId
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_ID)]
        OwinRequestId,

        /// <summary>
        /// owin.RequestMethod
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_METHOD)]
        OwinRequestMethod,

        /// <summary>
        /// owin.RequestPath
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_PATH)]
        OwinRequestPath,

        /// <summary>
        /// owin.RequestPathBase
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_PATH_BASE)]
        OwinRequestPathBase,

        /// <summary>
        /// owin.RequestProtocol
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_PROTOCOL)]
        OwinRequestProtocol,

        /// <summary>
        /// owin.RequestQueryString
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_QUERY_STRING)]
        OwinRequestQueryString,

        /// <summary>
        /// owin.RequestScheme
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_REQUEST_SCHEME)]
        OwinRequestScheme,

        /// <summary>
        /// owin.ResponseBody
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_RESPONSE_BODY)]
        OwinResponseBody,

        /// <summary>
        /// owin.ResponseHeaders
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_RESPONSE_HEADERS)]
        OwinResponseHeaders,
        /// <summary>
        /// owin.ResponseReasonPhrase
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_RESPONSE_REASON_PHRASE)]
        OwinResponseReasonPhrase,
        /// <summary>
        /// owin.ResponseStatusCode
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_RESPONSE_STATUS_CODE)]
        OwinResponseStatusCode,
        /// <summary>
        /// owin.Version
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.OWIN_VERSION)]
        OwinVersion,
        /// <summary>
        /// sendfile.SendAsync
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SEND_FILE_SEND_ASYNC)]
        SendfileSendAsync,
        /// <summary>
        /// server.Capabilities
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_CAPABILITIES)]
        ServerCapabilities,
        /// <summary>
        /// server.DisableRequestBuffering
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_DISABLE_REQUEST_BUFFERING)]
        ServerDisableRequestBuffering,
        /// <summary>
        /// server.DisableResponseBuffering
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_DISABLE_RESPONSE_BUFFERING)]
        ServerDisableResponseBuffering,
        /// <summary>
        /// server.IsLocal
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_IS_LOCAL)]
        ServerIsLocal,
        /// <summary>
        /// server.LocalIpAddress
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_LOCAL_IP_ADDRESS)]
        ServerLocalIpAddress,
        /// <summary>
        /// server.LocalPort
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_LOCAL_PORT)]
        ServerLocalPort,
        /// <summary>
        /// server.OnSendingHeaders
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_ON_SENDING_HEADERS)]
        ServerOnSendingHeaders,
        /// <summary>
        /// server.RemoteIpAddress
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_REMOTE_IP_ADDRESS)]
        ServerRemoteIpAddress,
        /// <summary>
        /// server.RemotePort
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_REMOTE_PORT)]
        ServerRemotePort,
        /// <summary>
        /// server.User
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SERVER_USER)]
        ServerUser,
        /// <summary>
        /// System.Web.HttpContextBase
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SYSTEM_WEB_HTTP_CONTEXT_BASE)]
        SystemWebHttpContextBase,
        /// <summary>
        /// System.Web.Routing.RequestContext
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SYSTEM_WEB_ROUTING_REQUEST_CONTEXT)]
        SystemWebRoutingRequestContext,
        /// <summary>
        /// systemweb.DisableResponseCompression
        /// </summary>
        [TitleEnum(OwinContextEnvironmentKeys.SYSTEM_WEB_DISABLE_RESPONSE_COMPRESSION)]
        SystemWebDisableResponseCompression,


    }


    /// <summary>
    /// Enum HttpServerVariablesKeyEnum
    /// </summary>
    public enum HttpServerVariablesKeyEnum
    {

        /// <summary>
        /// 未定义
        /// </summary>
        [TitleEnum("未定义")] UnDefine,

        /// <summary>
        /// 未知类型
        /// </summary>
        [TitleEnum("未知类型")] UnKnown,


        /// <summary>
        /// ALL_HTTP
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.ALL_HTTP)]
        AllHttp,
        /// <summary>
        /// ALL_RAW
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.ALL_RAW)]
        AllAaw,
        /// <summary>
        /// APPL_MD_PATH
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.APPL_MD_PATH)]
        ApplMdPath,
        /// <summary>
        /// APPL_PHYSICAL_PATH
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.APPL_PHYSICAL_PATH)]
        ApplPhysicalPath,
        /// <summary>
        /// AUTH_PASSWORD
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.AUTH_PASSWORD)]
        AuthPassword,
        /// <summary>
        /// AUTH_TYPE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.AUTH_TYPE)]
        AuthType,
        /// <summary>
        /// AUTH_USER
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.AUTH_USER)]
        AuthUser,
        /// <summary>
        /// CERT_COOKIE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_COOKIE)]
        CertCookie,
        /// <summary>
        /// CERT_FLAGS
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_FLAGS)]
        CertFlags,
        /// <summary>
        /// CERT_ISSUER
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_ISSUER)]
        CertIsSuer,
        /// <summary>
        /// CERT_KEYSIZE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_KEYSIZE)]
        CertKeySize,
        /// <summary>
        /// CERT_SECRETKEYSIZE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_SECRETKEYSIZE)]
        CertSecretKeySize,
        /// <summary>
        /// CERT_SERIALNUMBER
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_SERIALNUMBER)]
        CertSerialNumber,
        /// <summary>
        /// CERT_SERVER_ISSUER
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_SERVER_ISSUER)]
        CertServerIsSuer,
        /// <summary>
        /// CERT_SERVER_SUBJECT
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_SERVER_SUBJECT)]
        CertServerSubject,
        /// <summary>
        /// CERT_SUBJECT
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CERT_SUBJECT)]
        CertSubject,
        /// <summary>
        /// CONTENT_LENGTH
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CONTENT_LENGTH)]
        ContentLength,
        /// <summary>
        /// CONTENT_TYPE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.CONTENT_TYPE)]
        ContentType,
        /// <summary>
        /// GATEWAY_INTERFACE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.GATEWAY_INTERFACE)]
        GateWayInterface,
        /// <summary>
        /// HTTP_ACCEPT
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_ACCEPT)]
        HttpAccept,
        /// <summary>
        /// HTTP_ACCEPT_ENCODING
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_ACCEPT_ENCODING)]
        HttpAcceptEncoding,
        /// <summary>
        /// HTTP_ACCEPT_LANGUAGE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_ACCEPT_LANGUAGE)]
        HttpAcceptLanguage,
        /// <summary>
        /// HTTP_APPLICATIONINSIGHTS_REQUESTTRACKINGTELEMETRYMODULE_ROOTREQUEST_ID
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_APPLICATIONINSIGHTS_REQUESTTRACKINGTELEMETRYMODULE_ROOTREQUEST_ID)]
        HttpApplicationInsightsRequestTrackingTelemetryModuleRootRequestID,
        /// <summary>
        /// HTTP_CLIENT_IP
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_CLIENT_IP)]
        HttpClientIP,
        /// <summary>
        /// HTTP_CONNECTION
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_CONNECTION)]
        HttpConnection,
        /// <summary>
        /// HTTP_HOST
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_HOST)]
        HttpHost,
        /// <summary>
        /// HTTP_UPGRADE_INSECURE_REQUESTS
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_UPGRADE_INSECURE_REQUESTS)]
        HttpUpgradeInsecureRequests,
        /// <summary>
        /// HTTP_USER_AGENT
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_USER_AGENT)]
        HttpUserAgent,
        /// <summary>
        /// HTTP_X_FORWARDED_FOR
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTP_X_FORWARDED_FOR)]
        HttpXForwardedFor,
        /// <summary>
        /// HTTPS
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTPS)]
        Https,
        /// <summary>
        /// HTTPS_KEYSIZE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTPS_KEYSIZE)]
        HttpsKeySize,
        /// <summary>
        /// HTTPS_SECRETKEYSIZE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTPS_SECRETKEYSIZE)]
        HttpsSecretKeySize,
        /// <summary>
        /// HTTPS_SERVER_ISSUER
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTPS_SERVER_ISSUER)]
        HttpsServerIsSuer,
        /// <summary>
        /// HTTPS_SERVER_SUBJECT
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.HTTPS_SERVER_SUBJECT)]
        HttpsServerSubject,
        /// <summary>
        /// INSTANCE_ID
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.INSTANCE_ID)]
        InstanceID,
        /// <summary>
        /// INSTANCE_META_PATH
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.INSTANCE_META_PATH)]
        InstanceMetaPath,
        /// <summary>
        /// LOCAL_ADDR
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.LOCAL_ADDR)]
        LocalAddr,
        /// <summary>
        /// LOGON_USER
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.LOGON_USER)]
        LogonUser,
        /// <summary>
        /// PATH_INFO
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.PATH_INFO)]
        PathInfo,
        /// <summary>
        /// PATH_TRANSLATED
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.PATH_TRANSLATED)]
        PathTranslated,
        /// <summary>
        /// QUERY_STRING
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.QUERY_STRING)]
        QueryString,
        /// <summary>
        /// REMOTE_ADDR
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.REMOTE_ADDR)]
        RemoteAddr,
        /// <summary>
        /// REMOTE_HOST
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.REMOTE_HOST)]
        RemoteHost,
        /// <summary>
        /// REMOTE_PORT
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.REMOTE_PORT)]
        RemotePort,
        /// <summary>
        /// REMOTE_USER
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.REMOTE_USER)]
        RemoteUser,
        /// <summary>
        /// REQUEST_METHOD
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.REQUEST_METHOD)]
        RequestMethod,
        /// <summary>
        /// SCRIPT_NAME
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.SCRIPT_NAME)]
        ScriptName,
        /// <summary>
        /// SERVER_NAME
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.SERVER_NAME)]
        ServerName,
        /// <summary>
        /// SERVER_PORT
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.SERVER_PORT)]
        ServerPort,
        /// <summary>
        /// SERVER_PORT_SECURE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.SERVER_PORT_SECURE)]
        ServerPortSecure,
        /// <summary>
        /// SERVER_PROTOCOL
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.SERVER_PROTOCOL)]
        ServerProtocol,
        /// <summary>
        /// SERVER_SOFTWARE
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.SERVER_SOFTWARE)]
        ServerSoftware,
        /// <summary>
        /// URL
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.URL)]
        Url,
        /// <summary>
        /// WL-Proxy-Client-IP
        /// </summary>
        [TitleEnum(HttpServerVariablesKeys.WL_PROXY_CLIENT_IP)]
        WlProxyClientIp,




    }



    #endregion


    #region 文件后缀名枚举

    //public enum FileExtensionTypeEnum
    //{

    //    /// <summary>
    //    /// 未定义
    //    /// </summary>
    //    [FileExtensionEnum("未定义")]
    //    UnDefine,

    //    /// <summary>
    //    /// 未知类型
    //    /// </summary>
    //    [FileExtensionEnum("未知类型")]
    //    UnKnown,


    //    /// <summary>
    //    /// .a11
    //    /// </summary>
    //    [FileExtensionEnum(".a11", HttpContentTypeEnum.application_x_a11)]
    //    A11,


    //    /// <summary>
    //    /// .acp
    //    /// </summary>
    //    [FileExtensionEnum(".acp", HttpContentTypeEnum.audio_x_mei_aac)]
    //    Acp,


    //    /// <summary>
    //    /// .ai
    //    /// </summary>
    //    [FileExtensionEnum(".ai", HttpContentTypeEnum.application_postscript)]
    //    Ai,


    //    /// <summary>
    //    /// .aif
    //    /// </summary>
    //    [FileExtensionEnum(".aif", HttpContentTypeEnum.audio_aiff)]
    //    Aif,


    //    /// <summary>
    //    /// .aifc
    //    /// </summary>
    //    [FileExtensionEnum(".aifc", HttpContentTypeEnum.audio_aiff)]
    //    Aifc,


    //    /// <summary>
    //    /// .aiff
    //    /// </summary>
    //    [FileExtensionEnum(".aiff", HttpContentTypeEnum.audio_aiff)]
    //    Aiff,


    //    /// <summary>
    //    /// .anv
    //    /// </summary>
    //    [FileExtensionEnum(".anv", HttpContentTypeEnum.application_x_anv)]
    //    Anv,


    //    /// <summary>
    //    /// .apk
    //    /// </summary>
    //    [FileExtensionEnum(".apk", HttpContentTypeEnum.application_vnd_android_package_archive)]
    //    Apk,


    //    /// <summary>
    //    /// .asa
    //    /// </summary>
    //    [FileExtensionEnum(".asa", HttpContentTypeEnum.text_asa)]
    //    Asa,


    //    /// <summary>
    //    /// .asf
    //    /// </summary>
    //    [FileExtensionEnum(".asf", HttpContentTypeEnum.video_x_ms_asf)]
    //    Asf,


    //    /// <summary>
    //    /// .asp
    //    /// </summary>
    //    [FileExtensionEnum(".asp", HttpContentTypeEnum.text_asp)]
    //    Asp,


    //    /// <summary>
    //    /// .asx
    //    /// </summary>
    //    [FileExtensionEnum(".asx", HttpContentTypeEnum.video_x_ms_asf)]
    //    Asx,


    //    /// <summary>
    //    /// .au
    //    /// </summary>
    //    [FileExtensionEnum(".au", HttpContentTypeEnum.audio_basic)]
    //    Au,


    //    /// <summary>
    //    /// .avi
    //    /// </summary>
    //    [FileExtensionEnum(".avi", HttpContentTypeEnum.video_avi)]
    //    Avi,


    //    /// <summary>
    //    /// .awf
    //    /// </summary>
    //    [FileExtensionEnum(".awf", HttpContentTypeEnum.application_vnd_adobe_workflow)]
    //    Awf,


    //    /// <summary>
    //    /// .biz
    //    /// </summary>
    //    [FileExtensionEnum(".biz", HttpContentTypeEnum.text_xml)]
    //    Biz,


    //    /// <summary>
    //    /// .bmp
    //    /// </summary>
    //    [FileExtensionEnum(".bmp", HttpContentTypeEnum.application_x_bmp)]
    //    Bmp,


    //    /// <summary>
    //    /// .bot
    //    /// </summary>
    //    [FileExtensionEnum(".bot", HttpContentTypeEnum.application_x_bot)]
    //    Bot,


    //    /// <summary>
    //    /// .c4t
    //    /// </summary>
    //    [FileExtensionEnum(".c4t", HttpContentTypeEnum.application_x_c4t)]
    //    C4t,


    //    /// <summary>
    //    /// .c90
    //    /// </summary>
    //    [FileExtensionEnum(".c90", HttpContentTypeEnum.application_x_c90)]
    //    C90,


    //    /// <summary>
    //    /// .cal
    //    /// </summary>
    //    [FileExtensionEnum(".cal", HttpContentTypeEnum.application_x_cals)]
    //    Cal,


    //    /// <summary>
    //    /// .cat
    //    /// </summary>
    //    [FileExtensionEnum(".cat", HttpContentTypeEnum.application_vnd_ms_pki_seccat)]
    //    Cat,


    //    /// <summary>
    //    /// .cdf
    //    /// </summary>
    //    [FileExtensionEnum(".cdf", HttpContentTypeEnum.application_x_netcdf)]
    //    Cdf,


    //    /// <summary>
    //    /// .cdr
    //    /// </summary>
    //    [FileExtensionEnum(".cdr", HttpContentTypeEnum.application_x_cdr)]
    //    Cdr,


    //    /// <summary>
    //    /// .cel
    //    /// </summary>
    //    [FileExtensionEnum(".cel", HttpContentTypeEnum.application_x_cel)]
    //    Cel,


    //    /// <summary>
    //    /// .cer
    //    /// </summary>
    //    [FileExtensionEnum(".cer", HttpContentTypeEnum.application_x_x509_ca_cert)]
    //    Cer,


    //    /// <summary>
    //    /// .cg4
    //    /// </summary>
    //    [FileExtensionEnum(".cg4", HttpContentTypeEnum.application_x_g4)]
    //    Cg4,


    //    /// <summary>
    //    /// .cgm
    //    /// </summary>
    //    [FileExtensionEnum(".cgm", HttpContentTypeEnum.application_x_cgm)]
    //    Cgm,


    //    /// <summary>
    //    /// .cit
    //    /// </summary>
    //    [FileExtensionEnum(".cit", HttpContentTypeEnum.application_x_cit)]
    //    Cit,


    //    /// <summary>
    //    /// .class_java
    //    /// </summary>
    //    [FileExtensionEnum(".class_java", HttpContentTypeEnum.java_common)]
    //    Class_java,


    //    /// <summary>
    //    /// .cml
    //    /// </summary>
    //    [FileExtensionEnum(".cml", HttpContentTypeEnum.text_xml)]
    //    Cml,


    //    /// <summary>
    //    /// .cmp
    //    /// </summary>
    //    [FileExtensionEnum(".cmp", HttpContentTypeEnum.application_x_cmp)]
    //    Cmp,


    //    /// <summary>
    //    /// .cmx
    //    /// </summary>
    //    [FileExtensionEnum(".cmx", HttpContentTypeEnum.application_x_cmx)]
    //    Cmx,


    //    /// <summary>
    //    /// .cot
    //    /// </summary>
    //    [FileExtensionEnum(".cot", HttpContentTypeEnum.application_x_cot)]
    //    Cot,


    //    /// <summary>
    //    /// .crl
    //    /// </summary>
    //    [FileExtensionEnum(".crl", HttpContentTypeEnum.application_pkix_crl)]
    //    Crl,


    //    /// <summary>
    //    /// .crt
    //    /// </summary>
    //    [FileExtensionEnum(".crt", HttpContentTypeEnum.application_x_x509_ca_cert)]
    //    Crt,


    //    /// <summary>
    //    /// .csi
    //    /// </summary>
    //    [FileExtensionEnum(".csi", HttpContentTypeEnum.application_x_csi)]
    //    Csi,


    //    /// <summary>
    //    /// .css
    //    /// </summary>
    //    [FileExtensionEnum(".css", HttpContentTypeEnum.text_css)]
    //    Css,


    //    /// <summary>
    //    /// .cut
    //    /// </summary>
    //    [FileExtensionEnum(".cut", HttpContentTypeEnum.application_x_cut)]
    //    Cut,


    //    /// <summary>
    //    /// .dbf
    //    /// </summary>
    //    [FileExtensionEnum(".dbf", HttpContentTypeEnum.application_x_dbf)]
    //    Dbf,


    //    /// <summary>
    //    /// .dbm
    //    /// </summary>
    //    [FileExtensionEnum(".dbm", HttpContentTypeEnum.application_x_dbm)]
    //    Dbm,


    //    /// <summary>
    //    /// .dbx
    //    /// </summary>
    //    [FileExtensionEnum(".dbx", HttpContentTypeEnum.application_x_dbx)]
    //    Dbx,


    //    /// <summary>
    //    /// .dcd
    //    /// </summary>
    //    [FileExtensionEnum(".dcd", HttpContentTypeEnum.text_xml)]
    //    Dcd,


    //    /// <summary>
    //    /// .dcx
    //    /// </summary>
    //    [FileExtensionEnum(".dcx", HttpContentTypeEnum.application_x_dcx)]
    //    Dcx,


    //    /// <summary>
    //    /// .der
    //    /// </summary>
    //    [FileExtensionEnum(".der", HttpContentTypeEnum.application_x_x509_ca_cert)]
    //    Der,


    //    /// <summary>
    //    /// .dgn
    //    /// </summary>
    //    [FileExtensionEnum(".dgn", HttpContentTypeEnum.application_x_dgn)]
    //    Dgn,


    //    /// <summary>
    //    /// .dib
    //    /// </summary>
    //    [FileExtensionEnum(".dib", HttpContentTypeEnum.application_x_dib)]
    //    Dib,


    //    /// <summary>
    //    /// .dll
    //    /// </summary>
    //    [FileExtensionEnum(".dll", HttpContentTypeEnum.application_x_msdownload)]
    //    Dll,


    //    /// <summary>
    //    /// .doc
    //    /// </summary>
    //    [FileExtensionEnum(".doc", HttpContentTypeEnum.application_msword)]
    //    Doc,


    //    /// <summary>
    //    /// .dot
    //    /// </summary>
    //    [FileExtensionEnum(".dot", HttpContentTypeEnum.application_msword)]
    //    Dot,


    //    /// <summary>
    //    /// .drw
    //    /// </summary>
    //    [FileExtensionEnum(".drw", HttpContentTypeEnum.application_x_drw)]
    //    Drw,


    //    /// <summary>
    //    /// .dtd
    //    /// </summary>
    //    [FileExtensionEnum(".dtd", HttpContentTypeEnum.text_xml)]
    //    Dtd,


    //    /// <summary>
    //    /// .dwf
    //    /// </summary>
    //    [FileExtensionEnum(".dwf", HttpContentTypeEnum.application_x_dwf)]
    //    Dwf,


    //    /// <summary>
    //    /// .dwf
    //    /// </summary>
    //    [FileExtensionEnum(".dwf", HttpContentTypeEnum.model_vnd_dwf)]
    //    Dwf,


    //    /// <summary>
    //    /// .dwg
    //    /// </summary>
    //    [FileExtensionEnum(".dwg", HttpContentTypeEnum.application_x_dwg)]
    //    Dwg,


    //    /// <summary>
    //    /// .dxb
    //    /// </summary>
    //    [FileExtensionEnum(".dxb", HttpContentTypeEnum.application_x_dxb)]
    //    Dxb,


    //    /// <summary>
    //    /// .dxf
    //    /// </summary>
    //    [FileExtensionEnum(".dxf", HttpContentTypeEnum.application_x_dxf)]
    //    Dxf,


    //    /// <summary>
    //    /// .edn
    //    /// </summary>
    //    [FileExtensionEnum(".edn", HttpContentTypeEnum.application_vnd_adobe_edn)]
    //    Edn,


    //    /// <summary>
    //    /// .emf
    //    /// </summary>
    //    [FileExtensionEnum(".emf", HttpContentTypeEnum.application_x_emf)]
    //    Emf,


    //    /// <summary>
    //    /// .eml
    //    /// </summary>
    //    [FileExtensionEnum(".eml", HttpContentTypeEnum.message_rfc822)]
    //    Eml,


    //    /// <summary>
    //    /// .ent
    //    /// </summary>
    //    [FileExtensionEnum(".ent", HttpContentTypeEnum.text_xml)]
    //    Ent,


    //    /// <summary>
    //    /// .epi
    //    /// </summary>
    //    [FileExtensionEnum(".epi", HttpContentTypeEnum.application_x_epi)]
    //    Epi,


    //    /// <summary>
    //    /// .eps
    //    /// </summary>
    //    [FileExtensionEnum(".eps", HttpContentTypeEnum.application_postscript)]
    //    Eps,


    //    /// <summary>
    //    /// .eps
    //    /// </summary>
    //    [FileExtensionEnum(".eps", HttpContentTypeEnum.application_x_ps)]
    //    Eps,


    //    /// <summary>
    //    /// .etd
    //    /// </summary>
    //    [FileExtensionEnum(".etd", HttpContentTypeEnum.application_x_ebx)]
    //    Etd,


    //    /// <summary>
    //    /// .exe
    //    /// </summary>
    //    [FileExtensionEnum(".exe", HttpContentTypeEnum.application_x_msdownload)]
    //    Exe,


    //    /// <summary>
    //    /// .fax
    //    /// </summary>
    //    [FileExtensionEnum(".fax", HttpContentTypeEnum.image_fax)]
    //    Fax,


    //    /// <summary>
    //    /// .fdf
    //    /// </summary>
    //    [FileExtensionEnum(".fdf", HttpContentTypeEnum.application_vnd_fdf)]
    //    Fdf,


    //    /// <summary>
    //    /// .fif
    //    /// </summary>
    //    [FileExtensionEnum(".fif", HttpContentTypeEnum.application_fractals)]
    //    Fif,


    //    /// <summary>
    //    /// .fo
    //    /// </summary>
    //    [FileExtensionEnum(".fo", HttpContentTypeEnum.text_xml)]
    //    Fo,


    //    /// <summary>
    //    /// .frm
    //    /// </summary>
    //    [FileExtensionEnum(".frm", HttpContentTypeEnum.application_x_frm)]
    //    Frm,


    //    /// <summary>
    //    /// .g4
    //    /// </summary>
    //    [FileExtensionEnum(".g4", HttpContentTypeEnum.application_x_g4)]
    //    G4,


    //    /// <summary>
    //    /// .gbr
    //    /// </summary>
    //    [FileExtensionEnum(".gbr", HttpContentTypeEnum.application_x_gbr)]
    //    Gbr,


    //    /// <summary>
    //    /// .gif
    //    /// </summary>
    //    [FileExtensionEnum(".gif", HttpContentTypeEnum.image_gif)]
    //    Gif,


    //    /// <summary>
    //    /// .gl2
    //    /// </summary>
    //    [FileExtensionEnum(".gl2", HttpContentTypeEnum.application_x_gl2)]
    //    Gl2,


    //    /// <summary>
    //    /// .gp4
    //    /// </summary>
    //    [FileExtensionEnum(".gp4", HttpContentTypeEnum.application_x_gp4)]
    //    Gp4,


    //    /// <summary>
    //    /// .hgl
    //    /// </summary>
    //    [FileExtensionEnum(".hgl", HttpContentTypeEnum.application_x_hgl)]
    //    Hgl,


    //    /// <summary>
    //    /// .hmr
    //    /// </summary>
    //    [FileExtensionEnum(".hmr", HttpContentTypeEnum.application_x_hmr)]
    //    Hmr,


    //    /// <summary>
    //    /// .hpg
    //    /// </summary>
    //    [FileExtensionEnum(".hpg", HttpContentTypeEnum.application_x_hpgl)]
    //    Hpg,


    //    /// <summary>
    //    /// .hpl
    //    /// </summary>
    //    [FileExtensionEnum(".hpl", HttpContentTypeEnum.application_x_hpl)]
    //    Hpl,


    //    /// <summary>
    //    /// .hqx
    //    /// </summary>
    //    [FileExtensionEnum(".hqx", HttpContentTypeEnum.application_mac_binhex40)]
    //    Hqx,


    //    /// <summary>
    //    /// .hrf
    //    /// </summary>
    //    [FileExtensionEnum(".hrf", HttpContentTypeEnum.application_x_hrf)]
    //    Hrf,


    //    /// <summary>
    //    /// .hta
    //    /// </summary>
    //    [FileExtensionEnum(".hta", HttpContentTypeEnum.application_hta)]
    //    Hta,


    //    /// <summary>
    //    /// .htc
    //    /// </summary>
    //    [FileExtensionEnum(".htc", HttpContentTypeEnum.text_x_component)]
    //    Htc,


    //    /// <summary>
    //    /// .htm
    //    /// </summary>
    //    [FileExtensionEnum(".htm", HttpContentTypeEnum.text_html)]
    //    Htm,


    //    /// <summary>
    //    /// .html
    //    /// </summary>
    //    [FileExtensionEnum(".html", HttpContentTypeEnum.text_html)]
    //    Html,


    //    /// <summary>
    //    /// .htt
    //    /// </summary>
    //    [FileExtensionEnum(".htt", HttpContentTypeEnum.text_webviewhtml)]
    //    Htt,


    //    /// <summary>
    //    /// .htx
    //    /// </summary>
    //    [FileExtensionEnum(".htx", HttpContentTypeEnum.text_html)]
    //    Htx,


    //    /// <summary>
    //    /// .icb
    //    /// </summary>
    //    [FileExtensionEnum(".icb", HttpContentTypeEnum.application_x_icb)]
    //    Icb,


    //    /// <summary>
    //    /// .ico
    //    /// </summary>
    //    [FileExtensionEnum(".ico", HttpContentTypeEnum.application_x_ico)]
    //    Ico,


    //    /// <summary>
    //    /// .ico
    //    /// </summary>
    //    [FileExtensionEnum(".ico", HttpContentTypeEnum.image_x_icon)]
    //    Ico,


    //    /// <summary>
    //    /// .iff
    //    /// </summary>
    //    [FileExtensionEnum(".iff", HttpContentTypeEnum.application_x_iff)]
    //    Iff,


    //    /// <summary>
    //    /// .ig4
    //    /// </summary>
    //    [FileExtensionEnum(".ig4", HttpContentTypeEnum.application_x_g4)]
    //    Ig4,


    //    /// <summary>
    //    /// .igs
    //    /// </summary>
    //    [FileExtensionEnum(".igs", HttpContentTypeEnum.application_x_igs)]
    //    Igs,


    //    /// <summary>
    //    /// .iii
    //    /// </summary>
    //    [FileExtensionEnum(".iii", HttpContentTypeEnum.application_x_iphone)]
    //    Iii,


    //    /// <summary>
    //    /// .img
    //    /// </summary>
    //    [FileExtensionEnum(".img", HttpContentTypeEnum.application_x_img)]
    //    Img,


    //    /// <summary>
    //    /// .ins
    //    /// </summary>
    //    [FileExtensionEnum(".ins", HttpContentTypeEnum.application_x_internet_signup)]
    //    Ins,


    //    /// <summary>
    //    /// .ipa
    //    /// </summary>
    //    [FileExtensionEnum(".ipa", HttpContentTypeEnum.application_vnd_iphone)]
    //    Ipa,


    //    /// <summary>
    //    /// .isp
    //    /// </summary>
    //    [FileExtensionEnum(".isp", HttpContentTypeEnum.application_x_internet_signup)]
    //    Isp,


    //    /// <summary>
    //    /// .ivf
    //    /// </summary>
    //    [FileExtensionEnum(".ivf", HttpContentTypeEnum.video_x_ivf)]
    //    Ivf,


    //    /// <summary>
    //    /// .java
    //    /// </summary>
    //    [FileExtensionEnum(".java", HttpContentTypeEnum.java_common)]
    //    Java,


    //    /// <summary>
    //    /// .jfif
    //    /// </summary>
    //    [FileExtensionEnum(".jfif", HttpContentTypeEnum.image_jpeg)]
    //    Jfif,


    //    /// <summary>
    //    /// .jpe
    //    /// </summary>
    //    [FileExtensionEnum(".jpe", HttpContentTypeEnum.application_x_jpe)]
    //    Jpe,


    //    /// <summary>
    //    /// .jpe
    //    /// </summary>
    //    [FileExtensionEnum(".jpe", HttpContentTypeEnum.image_jpeg)]
    //    Jpe,


    //    /// <summary>
    //    /// .jpeg
    //    /// </summary>
    //    [FileExtensionEnum(".jpeg", HttpContentTypeEnum.image_jpeg)]
    //    Jpeg,


    //    /// <summary>
    //    /// .jpg
    //    /// </summary>
    //    [FileExtensionEnum(".jpg", HttpContentTypeEnum.application_x_jpg)]
    //    Jpg,


    //    /// <summary>
    //    /// .jpg
    //    /// </summary>
    //    [FileExtensionEnum(".jpg", HttpContentTypeEnum.image_jpeg)]
    //    Jpg,


    //    /// <summary>
    //    /// .js
    //    /// </summary>
    //    [FileExtensionEnum(".js", HttpContentTypeEnum.application_x_javascript)]
    //    Js,


    //    /// <summary>
    //    /// .jsp
    //    /// </summary>
    //    [FileExtensionEnum(".jsp", HttpContentTypeEnum.text_html)]
    //    Jsp,


    //    /// <summary>
    //    /// .la1
    //    /// </summary>
    //    [FileExtensionEnum(".la1", HttpContentTypeEnum.audio_x_liquid_file)]
    //    La1,


    //    /// <summary>
    //    /// .lar
    //    /// </summary>
    //    [FileExtensionEnum(".lar", HttpContentTypeEnum.application_x_laplayer_reg)]
    //    Lar,


    //    /// <summary>
    //    /// .latex
    //    /// </summary>
    //    [FileExtensionEnum(".latex", HttpContentTypeEnum.application_x_latex)]
    //    Latex,


    //    /// <summary>
    //    /// .lavs
    //    /// </summary>
    //    [FileExtensionEnum(".lavs", HttpContentTypeEnum.audio_x_liquid_secure)]
    //    Lavs,


    //    /// <summary>
    //    /// .lbm
    //    /// </summary>
    //    [FileExtensionEnum(".lbm", HttpContentTypeEnum.application_x_lbm)]
    //    Lbm,


    //    /// <summary>
    //    /// .lmsff
    //    /// </summary>
    //    [FileExtensionEnum(".lmsff", HttpContentTypeEnum.audio_x_la_lms)]
    //    Lmsff,


    //    /// <summary>
    //    /// .ls
    //    /// </summary>
    //    [FileExtensionEnum(".ls", HttpContentTypeEnum.application_x_javascript)]
    //    Ls,


    //    /// <summary>
    //    /// .ltr
    //    /// </summary>
    //    [FileExtensionEnum(".ltr", HttpContentTypeEnum.application_x_ltr)]
    //    Ltr,


    //    /// <summary>
    //    /// .m1v
    //    /// </summary>
    //    [FileExtensionEnum(".m1v", HttpContentTypeEnum.video_x_mpeg)]
    //    M1v,


    //    /// <summary>
    //    /// .m2v
    //    /// </summary>
    //    [FileExtensionEnum(".m2v", HttpContentTypeEnum.video_x_mpeg)]
    //    M2v,


    //    /// <summary>
    //    /// .m3u
    //    /// </summary>
    //    [FileExtensionEnum(".m3u", HttpContentTypeEnum.audio_mpegurl)]
    //    M3u,


    //    /// <summary>
    //    /// .m4e
    //    /// </summary>
    //    [FileExtensionEnum(".m4e", HttpContentTypeEnum.video_mpeg4)]
    //    M4e,


    //    /// <summary>
    //    /// .mac
    //    /// </summary>
    //    [FileExtensionEnum(".mac", HttpContentTypeEnum.application_x_mac)]
    //    Mac,


    //    /// <summary>
    //    /// .man
    //    /// </summary>
    //    [FileExtensionEnum(".man", HttpContentTypeEnum.application_x_troff_man)]
    //    Man,


    //    /// <summary>
    //    /// .math
    //    /// </summary>
    //    [FileExtensionEnum(".math", HttpContentTypeEnum.text_xml)]
    //    Math,


    //    /// <summary>
    //    /// .mdb
    //    /// </summary>
    //    [FileExtensionEnum(".mdb", HttpContentTypeEnum.application_msaccess)]
    //    Mdb,


    //    /// <summary>
    //    /// .mdb
    //    /// </summary>
    //    [FileExtensionEnum(".mdb", HttpContentTypeEnum.application_x_mdb)]
    //    Mdb,


    //    /// <summary>
    //    /// .mfp
    //    /// </summary>
    //    [FileExtensionEnum(".mfp", HttpContentTypeEnum.application_x_shockwave_flash)]
    //    Mfp,


    //    /// <summary>
    //    /// .mht
    //    /// </summary>
    //    [FileExtensionEnum(".mht", HttpContentTypeEnum.message_rfc822)]
    //    Mht,


    //    /// <summary>
    //    /// .mhtml
    //    /// </summary>
    //    [FileExtensionEnum(".mhtml", HttpContentTypeEnum.message_rfc822)]
    //    Mhtml,


    //    /// <summary>
    //    /// .mi
    //    /// </summary>
    //    [FileExtensionEnum(".mi", HttpContentTypeEnum.application_x_mi)]
    //    Mi,


    //    /// <summary>
    //    /// .mid
    //    /// </summary>
    //    [FileExtensionEnum(".mid", HttpContentTypeEnum.audio_mid)]
    //    Mid,


    //    /// <summary>
    //    /// .midi
    //    /// </summary>
    //    [FileExtensionEnum(".midi", HttpContentTypeEnum.audio_mid)]
    //    Midi,


    //    /// <summary>
    //    /// .mil
    //    /// </summary>
    //    [FileExtensionEnum(".mil", HttpContentTypeEnum.application_x_mil)]
    //    Mil,


    //    /// <summary>
    //    /// .mml
    //    /// </summary>
    //    [FileExtensionEnum(".mml", HttpContentTypeEnum.text_xml)]
    //    Mml,


    //    /// <summary>
    //    /// .mnd
    //    /// </summary>
    //    [FileExtensionEnum(".mnd", HttpContentTypeEnum.audio_x_musicnet_download)]
    //    Mnd,


    //    /// <summary>
    //    /// .mns
    //    /// </summary>
    //    [FileExtensionEnum(".mns", HttpContentTypeEnum.audio_x_musicnet_stream)]
    //    Mns,


    //    /// <summary>
    //    /// .mocha
    //    /// </summary>
    //    [FileExtensionEnum(".mocha", HttpContentTypeEnum.application_x_javascript)]
    //    Mocha,


    //    /// <summary>
    //    /// .movie
    //    /// </summary>
    //    [FileExtensionEnum(".movie", HttpContentTypeEnum.video_x_sgi_movie)]
    //    Movie,


    //    /// <summary>
    //    /// .mp1
    //    /// </summary>
    //    [FileExtensionEnum(".mp1", HttpContentTypeEnum.audio_mp1)]
    //    Mp1,


    //    /// <summary>
    //    /// .mp2
    //    /// </summary>
    //    [FileExtensionEnum(".mp2", HttpContentTypeEnum.audio_mp2)]
    //    Mp2,


    //    /// <summary>
    //    /// .mp2v
    //    /// </summary>
    //    [FileExtensionEnum(".mp2v", HttpContentTypeEnum.video_mpeg)]
    //    Mp2v,


    //    /// <summary>
    //    /// .mp3
    //    /// </summary>
    //    [FileExtensionEnum(".mp3", HttpContentTypeEnum.audio_mp3)]
    //    Mp3,


    //    /// <summary>
    //    /// .mp4
    //    /// </summary>
    //    [FileExtensionEnum(".mp4", HttpContentTypeEnum.video_mpeg4)]
    //    Mp4,


    //    /// <summary>
    //    /// .mpa
    //    /// </summary>
    //    [FileExtensionEnum(".mpa", HttpContentTypeEnum.video_x_mpg)]
    //    Mpa,


    //    /// <summary>
    //    /// .mpd
    //    /// </summary>
    //    [FileExtensionEnum(".mpd", HttpContentTypeEnum.application_vnd_ms_project)]
    //    Mpd,


    //    /// <summary>
    //    /// .mpe
    //    /// </summary>
    //    [FileExtensionEnum(".mpe", HttpContentTypeEnum.video_x_mpeg)]
    //    Mpe,


    //    /// <summary>
    //    /// .mpeg
    //    /// </summary>
    //    [FileExtensionEnum(".mpeg", HttpContentTypeEnum.video_mpg)]
    //    Mpeg,


    //    /// <summary>
    //    /// .mpg
    //    /// </summary>
    //    [FileExtensionEnum(".mpg", HttpContentTypeEnum.video_mpg)]
    //    Mpg,


    //    /// <summary>
    //    /// .mpga
    //    /// </summary>
    //    [FileExtensionEnum(".mpga", HttpContentTypeEnum.audio_rn_mpeg)]
    //    Mpga,


    //    /// <summary>
    //    /// .mpp
    //    /// </summary>
    //    [FileExtensionEnum(".mpp", HttpContentTypeEnum.application_vnd_ms_project)]
    //    Mpp,


    //    /// <summary>
    //    /// .mps
    //    /// </summary>
    //    [FileExtensionEnum(".mps", HttpContentTypeEnum.video_x_mpeg)]
    //    Mps,


    //    /// <summary>
    //    /// .mpt
    //    /// </summary>
    //    [FileExtensionEnum(".mpt", HttpContentTypeEnum.application_vnd_ms_project)]
    //    Mpt,


    //    /// <summary>
    //    /// .mpv
    //    /// </summary>
    //    [FileExtensionEnum(".mpv", HttpContentTypeEnum.video_mpg)]
    //    Mpv,


    //    /// <summary>
    //    /// .mpv2
    //    /// </summary>
    //    [FileExtensionEnum(".mpv2", HttpContentTypeEnum.video_mpeg)]
    //    Mpv2,


    //    /// <summary>
    //    /// .mpw
    //    /// </summary>
    //    [FileExtensionEnum(".mpw", HttpContentTypeEnum.application_vnd_ms_project)]
    //    Mpw,


    //    /// <summary>
    //    /// .mpx
    //    /// </summary>
    //    [FileExtensionEnum(".mpx", HttpContentTypeEnum.application_vnd_ms_project)]
    //    Mpx,


    //    /// <summary>
    //    /// .mtx
    //    /// </summary>
    //    [FileExtensionEnum(".mtx", HttpContentTypeEnum.text_xml)]
    //    Mtx,


    //    /// <summary>
    //    /// .mxp
    //    /// </summary>
    //    [FileExtensionEnum(".mxp", HttpContentTypeEnum.application_x_mmxp)]
    //    Mxp,


    //    /// <summary>
    //    /// .net
    //    /// </summary>
    //    [FileExtensionEnum(".net", HttpContentTypeEnum.image_pnetvue)]
    //    Net,


    //    /// <summary>
    //    /// .nrf
    //    /// </summary>
    //    [FileExtensionEnum(".nrf", HttpContentTypeEnum.application_x_nrf)]
    //    Nrf,


    //    /// <summary>
    //    /// .nws
    //    /// </summary>
    //    [FileExtensionEnum(".nws", HttpContentTypeEnum.message_rfc822)]
    //    Nws,


    //    /// <summary>
    //    /// .odc
    //    /// </summary>
    //    [FileExtensionEnum(".odc", HttpContentTypeEnum.text_x_ms_odc)]
    //    Odc,


    //    /// <summary>
    //    /// .out
    //    /// </summary>
    //    [FileExtensionEnum(".out", HttpContentTypeEnum.application_x_out)]
    //    Out,


    //    /// <summary>
    //    /// .p10
    //    /// </summary>
    //    [FileExtensionEnum(".p10", HttpContentTypeEnum.application_pkcs10)]
    //    P10,


    //    /// <summary>
    //    /// .p12
    //    /// </summary>
    //    [FileExtensionEnum(".p12", HttpContentTypeEnum.application_x_pkcs12)]
    //    P12,


    //    /// <summary>
    //    /// .p7b
    //    /// </summary>
    //    [FileExtensionEnum(".p7b", HttpContentTypeEnum.application_x_pkcs7_certificates)]
    //    P7b,


    //    /// <summary>
    //    /// .p7c
    //    /// </summary>
    //    [FileExtensionEnum(".p7c", HttpContentTypeEnum.application_pkcs7_mime)]
    //    P7c,


    //    /// <summary>
    //    /// .p7m
    //    /// </summary>
    //    [FileExtensionEnum(".p7m", HttpContentTypeEnum.application_pkcs7_mime)]
    //    P7m,


    //    /// <summary>
    //    /// .p7r
    //    /// </summary>
    //    [FileExtensionEnum(".p7r", HttpContentTypeEnum.application_x_pkcs7_certreqresp)]
    //    P7r,


    //    /// <summary>
    //    /// .p7s
    //    /// </summary>
    //    [FileExtensionEnum(".p7s", HttpContentTypeEnum.application_pkcs7_signature)]
    //    P7s,


    //    /// <summary>
    //    /// .pc5
    //    /// </summary>
    //    [FileExtensionEnum(".pc5", HttpContentTypeEnum.application_x_pc5)]
    //    Pc5,


    //    /// <summary>
    //    /// .pci
    //    /// </summary>
    //    [FileExtensionEnum(".pci", HttpContentTypeEnum.application_x_pci)]
    //    Pci,


    //    /// <summary>
    //    /// .pcl
    //    /// </summary>
    //    [FileExtensionEnum(".pcl", HttpContentTypeEnum.application_x_pcl)]
    //    Pcl,


    //    /// <summary>
    //    /// .pcx
    //    /// </summary>
    //    [FileExtensionEnum(".pcx", HttpContentTypeEnum.application_x_pcx)]
    //    Pcx,


    //    /// <summary>
    //    /// .pdf
    //    /// </summary>
    //    [FileExtensionEnum(".pdf", HttpContentTypeEnum.application_pdf)]
    //    Pdf,


    //    /// <summary>
    //    /// .pdf
    //    /// </summary>
    //    [FileExtensionEnum(".pdf", HttpContentTypeEnum.application_pdf)]
    //    Pdf,


    //    /// <summary>
    //    /// .pdx
    //    /// </summary>
    //    [FileExtensionEnum(".pdx", HttpContentTypeEnum.application_vnd_adobe_pdx)]
    //    Pdx,


    //    /// <summary>
    //    /// .pfx
    //    /// </summary>
    //    [FileExtensionEnum(".pfx", HttpContentTypeEnum.application_x_pkcs12)]
    //    Pfx,


    //    /// <summary>
    //    /// .pgl
    //    /// </summary>
    //    [FileExtensionEnum(".pgl", HttpContentTypeEnum.application_x_pgl)]
    //    Pgl,


    //    /// <summary>
    //    /// .pic
    //    /// </summary>
    //    [FileExtensionEnum(".pic", HttpContentTypeEnum.application_x_pic)]
    //    Pic,


    //    /// <summary>
    //    /// .pko
    //    /// </summary>
    //    [FileExtensionEnum(".pko", HttpContentTypeEnum.application_vnd_ms_pki_pko)]
    //    Pko,


    //    /// <summary>
    //    /// .pl
    //    /// </summary>
    //    [FileExtensionEnum(".pl", HttpContentTypeEnum.application_x_perl)]
    //    Pl,


    //    /// <summary>
    //    /// .plg
    //    /// </summary>
    //    [FileExtensionEnum(".plg", HttpContentTypeEnum.text_html)]
    //    Plg,


    //    /// <summary>
    //    /// .pls
    //    /// </summary>
    //    [FileExtensionEnum(".pls", HttpContentTypeEnum.audio_scpls)]
    //    Pls,


    //    /// <summary>
    //    /// .plt
    //    /// </summary>
    //    [FileExtensionEnum(".plt", HttpContentTypeEnum.application_x_plt)]
    //    Plt,


    //    /// <summary>
    //    /// .png
    //    /// </summary>
    //    [FileExtensionEnum(".png", HttpContentTypeEnum.application_x_png)]
    //    Png,


    //    /// <summary>
    //    /// .png
    //    /// </summary>
    //    [FileExtensionEnum(".png", HttpContentTypeEnum.image_png)]
    //    Png,


    //    /// <summary>
    //    /// .pot
    //    /// </summary>
    //    [FileExtensionEnum(".pot", HttpContentTypeEnum.application_vnd_ms_powerpoint)]
    //    Pot,


    //    /// <summary>
    //    /// .ppa
    //    /// </summary>
    //    [FileExtensionEnum(".ppa", HttpContentTypeEnum.application_vnd_ms_powerpoint)]
    //    Ppa,


    //    /// <summary>
    //    /// .ppm
    //    /// </summary>
    //    [FileExtensionEnum(".ppm", HttpContentTypeEnum.application_x_ppm)]
    //    Ppm,


    //    /// <summary>
    //    /// .pps
    //    /// </summary>
    //    [FileExtensionEnum(".pps", HttpContentTypeEnum.application_vnd_ms_powerpoint)]
    //    Pps,


    //    /// <summary>
    //    /// .ppt
    //    /// </summary>
    //    [FileExtensionEnum(".ppt", HttpContentTypeEnum.application_vnd_ms_powerpoint)]
    //    Ppt,


    //    /// <summary>
    //    /// .ppt
    //    /// </summary>
    //    [FileExtensionEnum(".ppt", HttpContentTypeEnum.application_x_ppt)]
    //    Ppt,


    //    /// <summary>
    //    /// .pr
    //    /// </summary>
    //    [FileExtensionEnum(".pr", HttpContentTypeEnum.application_x_pr)]
    //    Pr,


    //    /// <summary>
    //    /// .prf
    //    /// </summary>
    //    [FileExtensionEnum(".prf", HttpContentTypeEnum.application_pics_rules)]
    //    Prf,


    //    /// <summary>
    //    /// .prn
    //    /// </summary>
    //    [FileExtensionEnum(".prn", HttpContentTypeEnum.application_x_prn)]
    //    Prn,


    //    /// <summary>
    //    /// .prt
    //    /// </summary>
    //    [FileExtensionEnum(".prt", HttpContentTypeEnum.application_x_prt)]
    //    Prt,


    //    /// <summary>
    //    /// .ps
    //    /// </summary>
    //    [FileExtensionEnum(".ps", HttpContentTypeEnum.application_postscript)]
    //    Ps,


    //    /// <summary>
    //    /// .ps
    //    /// </summary>
    //    [FileExtensionEnum(".ps", HttpContentTypeEnum.application_x_ps)]
    //    Ps,


    //    /// <summary>
    //    /// .ptn
    //    /// </summary>
    //    [FileExtensionEnum(".ptn", HttpContentTypeEnum.application_x_ptn)]
    //    Ptn,


    //    /// <summary>
    //    /// .pwz
    //    /// </summary>
    //    [FileExtensionEnum(".pwz", HttpContentTypeEnum.application_vnd_ms_powerpoint)]
    //    Pwz,


    //    /// <summary>
    //    /// .r3t
    //    /// </summary>
    //    [FileExtensionEnum(".r3t", HttpContentTypeEnum.text_vnd_rn_realtext3d)]
    //    R3t,


    //    /// <summary>
    //    /// .ra
    //    /// </summary>
    //    [FileExtensionEnum(".ra", HttpContentTypeEnum.audio_vnd_rn_realaudio)]
    //    Ra,


    //    /// <summary>
    //    /// .ram
    //    /// </summary>
    //    [FileExtensionEnum(".ram", HttpContentTypeEnum.audio_x_pn_realaudio)]
    //    Ram,


    //    /// <summary>
    //    /// .ras
    //    /// </summary>
    //    [FileExtensionEnum(".ras", HttpContentTypeEnum.application_x_ras)]
    //    Ras,


    //    /// <summary>
    //    /// .rat
    //    /// </summary>
    //    [FileExtensionEnum(".rat", HttpContentTypeEnum.application_rat_file)]
    //    Rat,


    //    /// <summary>
    //    /// .rdf
    //    /// </summary>
    //    [FileExtensionEnum(".rdf", HttpContentTypeEnum.text_xml)]
    //    Rdf,


    //    /// <summary>
    //    /// .rec
    //    /// </summary>
    //    [FileExtensionEnum(".rec", HttpContentTypeEnum.application_vnd_rn_recording)]
    //    Rec,


    //    /// <summary>
    //    /// .red
    //    /// </summary>
    //    [FileExtensionEnum(".red", HttpContentTypeEnum.application_x_red)]
    //    Red,


    //    /// <summary>
    //    /// .rgb
    //    /// </summary>
    //    [FileExtensionEnum(".rgb", HttpContentTypeEnum.application_x_rgb)]
    //    Rgb,


    //    /// <summary>
    //    /// .rjs
    //    /// </summary>
    //    [FileExtensionEnum(".rjs", HttpContentTypeEnum.application_vnd_rn_realsystem_rjs)]
    //    Rjs,


    //    /// <summary>
    //    /// .rjt
    //    /// </summary>
    //    [FileExtensionEnum(".rjt", HttpContentTypeEnum.application_vnd_rn_realsystem_rjt)]
    //    Rjt,


    //    /// <summary>
    //    /// .rlc
    //    /// </summary>
    //    [FileExtensionEnum(".rlc", HttpContentTypeEnum.application_x_rlc)]
    //    Rlc,


    //    /// <summary>
    //    /// .rle
    //    /// </summary>
    //    [FileExtensionEnum(".rle", HttpContentTypeEnum.application_x_rle)]
    //    Rle,


    //    /// <summary>
    //    /// .rm
    //    /// </summary>
    //    [FileExtensionEnum(".rm", HttpContentTypeEnum.application_vnd_rn_realmedia)]
    //    Rm,


    //    /// <summary>
    //    /// .rmf
    //    /// </summary>
    //    [FileExtensionEnum(".rmf", HttpContentTypeEnum.application_vnd_adobe_rmf)]
    //    Rmf,


    //    /// <summary>
    //    /// .rmi
    //    /// </summary>
    //    [FileExtensionEnum(".rmi", HttpContentTypeEnum.audio_mid)]
    //    Rmi,


    //    /// <summary>
    //    /// .rmj
    //    /// </summary>
    //    [FileExtensionEnum(".rmj", HttpContentTypeEnum.application_vnd_rn_realsystem_rmj)]
    //    Rmj,


    //    /// <summary>
    //    /// .rmm
    //    /// </summary>
    //    [FileExtensionEnum(".rmm", HttpContentTypeEnum.audio_x_pn_realaudio)]
    //    Rmm,


    //    /// <summary>
    //    /// .rmp
    //    /// </summary>
    //    [FileExtensionEnum(".rmp", HttpContentTypeEnum.application_vnd_rn_rn_music_package)]
    //    Rmp,


    //    /// <summary>
    //    /// .rms
    //    /// </summary>
    //    [FileExtensionEnum(".rms", HttpContentTypeEnum.application_vnd_rn_realmedia_secure)]
    //    Rms,


    //    /// <summary>
    //    /// .rmvb
    //    /// </summary>
    //    [FileExtensionEnum(".rmvb", HttpContentTypeEnum.application_vnd_rn_realmedia_vbr)]
    //    Rmvb,


    //    /// <summary>
    //    /// .rmx
    //    /// </summary>
    //    [FileExtensionEnum(".rmx", HttpContentTypeEnum.application_vnd_rn_realsystem_rmx)]
    //    Rmx,


    //    /// <summary>
    //    /// .rnx
    //    /// </summary>
    //    [FileExtensionEnum(".rnx", HttpContentTypeEnum.application_vnd_rn_realplayer)]
    //    Rnx,


    //    /// <summary>
    //    /// .rp
    //    /// </summary>
    //    [FileExtensionEnum(".rp", HttpContentTypeEnum.image_vnd_rn_realpix)]
    //    Rp,


    //    /// <summary>
    //    /// .rpm
    //    /// </summary>
    //    [FileExtensionEnum(".rpm", HttpContentTypeEnum.audio_x_pn_realaudio_plugin)]
    //    Rpm,


    //    /// <summary>
    //    /// .rsml
    //    /// </summary>
    //    [FileExtensionEnum(".rsml", HttpContentTypeEnum.application_vnd_rn_rsml)]
    //    Rsml,


    //    /// <summary>
    //    /// .rt
    //    /// </summary>
    //    [FileExtensionEnum(".rt", HttpContentTypeEnum.text_vnd_rn_realtext)]
    //    Rt,


    //    /// <summary>
    //    /// .rtf
    //    /// </summary>
    //    [FileExtensionEnum(".rtf", HttpContentTypeEnum.application_msword)]
    //    Rtf,


    //    /// <summary>
    //    /// .rtf
    //    /// </summary>
    //    [FileExtensionEnum(".rtf", HttpContentTypeEnum.application_x_rtf)]
    //    Rtf,


    //    /// <summary>
    //    /// .rv
    //    /// </summary>
    //    [FileExtensionEnum(".rv", HttpContentTypeEnum.video_vnd_rn_realvideo)]
    //    Rv,


    //    /// <summary>
    //    /// .sam
    //    /// </summary>
    //    [FileExtensionEnum(".sam", HttpContentTypeEnum.application_x_sam)]
    //    Sam,


    //    /// <summary>
    //    /// .sat
    //    /// </summary>
    //    [FileExtensionEnum(".sat", HttpContentTypeEnum.application_x_sat)]
    //    Sat,


    //    /// <summary>
    //    /// .sdp
    //    /// </summary>
    //    [FileExtensionEnum(".sdp", HttpContentTypeEnum.application_sdp)]
    //    Sdp,


    //    /// <summary>
    //    /// .sdw
    //    /// </summary>
    //    [FileExtensionEnum(".sdw", HttpContentTypeEnum.application_x_sdw)]
    //    Sdw,


    //    /// <summary>
    //    /// .sis
    //    /// </summary>
    //    [FileExtensionEnum(".sis", HttpContentTypeEnum.application_vnd_symbian_install)]
    //    Sis,


    //    /// <summary>
    //    /// .sisx
    //    /// </summary>
    //    [FileExtensionEnum(".sisx", HttpContentTypeEnum.application_vnd_symbian_install)]
    //    Sisx,


    //    /// <summary>
    //    /// .sit
    //    /// </summary>
    //    [FileExtensionEnum(".sit", HttpContentTypeEnum.application_x_stuffit)]
    //    Sit,


    //    /// <summary>
    //    /// .slb
    //    /// </summary>
    //    [FileExtensionEnum(".slb", HttpContentTypeEnum.application_x_slb)]
    //    Slb,


    //    /// <summary>
    //    /// .sld
    //    /// </summary>
    //    [FileExtensionEnum(".sld", HttpContentTypeEnum.application_x_sld)]
    //    Sld,


    //    /// <summary>
    //    /// .slk
    //    /// </summary>
    //    [FileExtensionEnum(".slk", HttpContentTypeEnum.drawing_x_slk)]
    //    Slk,


    //    /// <summary>
    //    /// .smi
    //    /// </summary>
    //    [FileExtensionEnum(".smi", HttpContentTypeEnum.application_smil)]
    //    Smi,


    //    /// <summary>
    //    /// .smil
    //    /// </summary>
    //    [FileExtensionEnum(".smil", HttpContentTypeEnum.application_smil)]
    //    Smil,


    //    /// <summary>
    //    /// .smk
    //    /// </summary>
    //    [FileExtensionEnum(".smk", HttpContentTypeEnum.application_x_smk)]
    //    Smk,


    //    /// <summary>
    //    /// .snd
    //    /// </summary>
    //    [FileExtensionEnum(".snd", HttpContentTypeEnum.audio_basic)]
    //    Snd,


    //    /// <summary>
    //    /// .sol
    //    /// </summary>
    //    [FileExtensionEnum(".sol", HttpContentTypeEnum.text_plain)]
    //    Sol,


    //    /// <summary>
    //    /// .sor
    //    /// </summary>
    //    [FileExtensionEnum(".sor", HttpContentTypeEnum.text_plain)]
    //    Sor,


    //    /// <summary>
    //    /// .spc
    //    /// </summary>
    //    [FileExtensionEnum(".spc", HttpContentTypeEnum.application_x_pkcs7_certificates)]
    //    Spc,


    //    /// <summary>
    //    /// .spl
    //    /// </summary>
    //    [FileExtensionEnum(".spl", HttpContentTypeEnum.application_futuresplash)]
    //    Spl,


    //    /// <summary>
    //    /// .spp
    //    /// </summary>
    //    [FileExtensionEnum(".spp", HttpContentTypeEnum.text_xml)]
    //    Spp,


    //    /// <summary>
    //    /// .ssm
    //    /// </summary>
    //    [FileExtensionEnum(".ssm", HttpContentTypeEnum.application_streamingmedia)]
    //    Ssm,


    //    /// <summary>
    //    /// .sst
    //    /// </summary>
    //    [FileExtensionEnum(".sst", HttpContentTypeEnum.application_vnd_ms_pki_certstore)]
    //    Sst,


    //    /// <summary>
    //    /// .stl
    //    /// </summary>
    //    [FileExtensionEnum(".stl", HttpContentTypeEnum.application_vnd_ms_pki_stl)]
    //    Stl,


    //    /// <summary>
    //    /// .stm
    //    /// </summary>
    //    [FileExtensionEnum(".stm", HttpContentTypeEnum.text_html)]
    //    Stm,


    //    /// <summary>
    //    /// .sty
    //    /// </summary>
    //    [FileExtensionEnum(".sty", HttpContentTypeEnum.application_x_sty)]
    //    Sty,


    //    /// <summary>
    //    /// .svg
    //    /// </summary>
    //    [FileExtensionEnum(".svg", HttpContentTypeEnum.text_xml)]
    //    Svg,


    //    /// <summary>
    //    /// .swf
    //    /// </summary>
    //    [FileExtensionEnum(".swf", HttpContentTypeEnum.application_x_shockwave_flash)]
    //    Swf,


    //    /// <summary>
    //    /// .tdf
    //    /// </summary>
    //    [FileExtensionEnum(".tdf", HttpContentTypeEnum.application_x_tdf)]
    //    Tdf,


    //    /// <summary>
    //    /// .tg4
    //    /// </summary>
    //    [FileExtensionEnum(".tg4", HttpContentTypeEnum.application_x_tg4)]
    //    Tg4,


    //    /// <summary>
    //    /// .tga
    //    /// </summary>
    //    [FileExtensionEnum(".tga", HttpContentTypeEnum.application_x_tga)]
    //    Tga,


    //    /// <summary>
    //    /// .tif
    //    /// </summary>
    //    [FileExtensionEnum(".tif", HttpContentTypeEnum.application_x_tif)]
    //    Tif,


    //    /// <summary>
    //    /// .tif
    //    /// </summary>
    //    [FileExtensionEnum(".tif", HttpContentTypeEnum.image_tiff)]
    //    Tif,


    //    /// <summary>
    //    /// .tif
    //    /// </summary>
    //    [FileExtensionEnum(".tif", HttpContentTypeEnum.image_tiff)]
    //    Tif,


    //    /// <summary>
    //    /// .tiff
    //    /// </summary>
    //    [FileExtensionEnum(".tiff", HttpContentTypeEnum.image_tiff)]
    //    Tiff,


    //    /// <summary>
    //    /// .tld
    //    /// </summary>
    //    [FileExtensionEnum(".tld", HttpContentTypeEnum.text_xml)]
    //    Tld,


    //    /// <summary>
    //    /// .top
    //    /// </summary>
    //    [FileExtensionEnum(".top", HttpContentTypeEnum.drawing_x_top)]
    //    Top,


    //    /// <summary>
    //    /// .torrent
    //    /// </summary>
    //    [FileExtensionEnum(".torrent", HttpContentTypeEnum.application_x_bittorrent)]
    //    Torrent,


    //    /// <summary>
    //    /// .tsd
    //    /// </summary>
    //    [FileExtensionEnum(".tsd", HttpContentTypeEnum.text_xml)]
    //    Tsd,


    //    /// <summary>
    //    /// .txt
    //    /// </summary>
    //    [FileExtensionEnum(".txt", HttpContentTypeEnum.text_plain)]
    //    Txt,


    //    /// <summary>
    //    /// .uin
    //    /// </summary>
    //    [FileExtensionEnum(".uin", HttpContentTypeEnum.application_x_icq)]
    //    Uin,


    //    /// <summary>
    //    /// .uls
    //    /// </summary>
    //    [FileExtensionEnum(".uls", HttpContentTypeEnum.text_iuls)]
    //    Uls,


    //    /// <summary>
    //    /// .vcf
    //    /// </summary>
    //    [FileExtensionEnum(".vcf", HttpContentTypeEnum.text_x_vcard)]
    //    Vcf,


    //    /// <summary>
    //    /// .vda
    //    /// </summary>
    //    [FileExtensionEnum(".vda", HttpContentTypeEnum.application_x_vda)]
    //    Vda,


    //    /// <summary>
    //    /// .vdx
    //    /// </summary>
    //    [FileExtensionEnum(".vdx", HttpContentTypeEnum.application_vnd_visio)]
    //    Vdx,


    //    /// <summary>
    //    /// .vml
    //    /// </summary>
    //    [FileExtensionEnum(".vml", HttpContentTypeEnum.text_xml)]
    //    Vml,


    //    /// <summary>
    //    /// .vpg
    //    /// </summary>
    //    [FileExtensionEnum(".vpg", HttpContentTypeEnum.application_x_vpeg005)]
    //    Vpg,


    //    /// <summary>
    //    /// .vsd
    //    /// </summary>
    //    [FileExtensionEnum(".vsd", HttpContentTypeEnum.application_vnd_visio)]
    //    Vsd,


    //    /// <summary>
    //    /// .vsd
    //    /// </summary>
    //    [FileExtensionEnum(".vsd", HttpContentTypeEnum.application_x_vsd)]
    //    Vsd,


    //    /// <summary>
    //    /// .vss
    //    /// </summary>
    //    [FileExtensionEnum(".vss", HttpContentTypeEnum.application_vnd_visio)]
    //    Vss,


    //    /// <summary>
    //    /// .vst
    //    /// </summary>
    //    [FileExtensionEnum(".vst", HttpContentTypeEnum.application_vnd_visio)]
    //    Vst,


    //    /// <summary>
    //    /// .vst
    //    /// </summary>
    //    [FileExtensionEnum(".vst", HttpContentTypeEnum.application_x_vst)]
    //    Vst,


    //    /// <summary>
    //    /// .vsw
    //    /// </summary>
    //    [FileExtensionEnum(".vsw", HttpContentTypeEnum.application_vnd_visio)]
    //    Vsw,


    //    /// <summary>
    //    /// .vsx
    //    /// </summary>
    //    [FileExtensionEnum(".vsx", HttpContentTypeEnum.application_vnd_visio)]
    //    Vsx,


    //    /// <summary>
    //    /// .vtx
    //    /// </summary>
    //    [FileExtensionEnum(".vtx", HttpContentTypeEnum.application_vnd_visio)]
    //    Vtx,


    //    /// <summary>
    //    /// .vxml
    //    /// </summary>
    //    [FileExtensionEnum(".vxml", HttpContentTypeEnum.text_xml)]
    //    Vxml,


    //    /// <summary>
    //    /// .wav
    //    /// </summary>
    //    [FileExtensionEnum(".wav", HttpContentTypeEnum.audio_wav)]
    //    Wav,


    //    /// <summary>
    //    /// .wax
    //    /// </summary>
    //    [FileExtensionEnum(".wax", HttpContentTypeEnum.audio_x_ms_wax)]
    //    Wax,


    //    /// <summary>
    //    /// .wb1
    //    /// </summary>
    //    [FileExtensionEnum(".wb1", HttpContentTypeEnum.application_x_wb1)]
    //    Wb1,


    //    /// <summary>
    //    /// .wb2
    //    /// </summary>
    //    [FileExtensionEnum(".wb2", HttpContentTypeEnum.application_x_wb2)]
    //    Wb2,


    //    /// <summary>
    //    /// .wb3
    //    /// </summary>
    //    [FileExtensionEnum(".wb3", HttpContentTypeEnum.application_x_wb3)]
    //    Wb3,


    //    /// <summary>
    //    /// .wbmp
    //    /// </summary>
    //    [FileExtensionEnum(".wbmp", HttpContentTypeEnum.image_vnd_wap_wbmp)]
    //    Wbmp,


    //    /// <summary>
    //    /// .wiz
    //    /// </summary>
    //    [FileExtensionEnum(".wiz", HttpContentTypeEnum.application_msword)]
    //    Wiz,


    //    /// <summary>
    //    /// .wk3
    //    /// </summary>
    //    [FileExtensionEnum(".wk3", HttpContentTypeEnum.application_x_wk3)]
    //    Wk3,


    //    /// <summary>
    //    /// .wk4
    //    /// </summary>
    //    [FileExtensionEnum(".wk4", HttpContentTypeEnum.application_x_wk4)]
    //    Wk4,


    //    /// <summary>
    //    /// .wkq
    //    /// </summary>
    //    [FileExtensionEnum(".wkq", HttpContentTypeEnum.application_x_wkq)]
    //    Wkq,


    //    /// <summary>
    //    /// .wks
    //    /// </summary>
    //    [FileExtensionEnum(".wks", HttpContentTypeEnum.application_x_wks)]
    //    Wks,


    //    /// <summary>
    //    /// .wm
    //    /// </summary>
    //    [FileExtensionEnum(".wm", HttpContentTypeEnum.video_x_ms_wm)]
    //    Wm,


    //    /// <summary>
    //    /// .wma
    //    /// </summary>
    //    [FileExtensionEnum(".wma", HttpContentTypeEnum.audio_x_ms_wma)]
    //    Wma,


    //    /// <summary>
    //    /// .wmd
    //    /// </summary>
    //    [FileExtensionEnum(".wmd", HttpContentTypeEnum.application_x_ms_wmd)]
    //    Wmd,


    //    /// <summary>
    //    /// .wmf
    //    /// </summary>
    //    [FileExtensionEnum(".wmf", HttpContentTypeEnum.application_x_wmf)]
    //    Wmf,


    //    /// <summary>
    //    /// .wml
    //    /// </summary>
    //    [FileExtensionEnum(".wml", HttpContentTypeEnum.text_vnd_wap_wml)]
    //    Wml,


    //    /// <summary>
    //    /// .wmv
    //    /// </summary>
    //    [FileExtensionEnum(".wmv", HttpContentTypeEnum.video_x_ms_wmv)]
    //    Wmv,


    //    /// <summary>
    //    /// .wmx
    //    /// </summary>
    //    [FileExtensionEnum(".wmx", HttpContentTypeEnum.video_x_ms_wmx)]
    //    Wmx,


    //    /// <summary>
    //    /// .wmz
    //    /// </summary>
    //    [FileExtensionEnum(".wmz", HttpContentTypeEnum.application_x_ms_wmz)]
    //    Wmz,


    //    /// <summary>
    //    /// .wp6
    //    /// </summary>
    //    [FileExtensionEnum(".wp6", HttpContentTypeEnum.application_x_wp6)]
    //    Wp6,


    //    /// <summary>
    //    /// .wpd
    //    /// </summary>
    //    [FileExtensionEnum(".wpd", HttpContentTypeEnum.application_x_wpd)]
    //    Wpd,


    //    /// <summary>
    //    /// .wpg
    //    /// </summary>
    //    [FileExtensionEnum(".wpg", HttpContentTypeEnum.application_x_wpg)]
    //    Wpg,


    //    /// <summary>
    //    /// .wpl
    //    /// </summary>
    //    [FileExtensionEnum(".wpl", HttpContentTypeEnum.application_vnd_ms_wpl)]
    //    Wpl,


    //    /// <summary>
    //    /// .wq1
    //    /// </summary>
    //    [FileExtensionEnum(".wq1", HttpContentTypeEnum.application_x_wq1)]
    //    Wq1,


    //    /// <summary>
    //    /// .wr1
    //    /// </summary>
    //    [FileExtensionEnum(".wr1", HttpContentTypeEnum.application_x_wr1)]
    //    Wr1,


    //    /// <summary>
    //    /// .wri
    //    /// </summary>
    //    [FileExtensionEnum(".wri", HttpContentTypeEnum.application_x_wri)]
    //    Wri,


    //    /// <summary>
    //    /// .wrk
    //    /// </summary>
    //    [FileExtensionEnum(".wrk", HttpContentTypeEnum.application_x_wrk)]
    //    Wrk,


    //    /// <summary>
    //    /// .ws
    //    /// </summary>
    //    [FileExtensionEnum(".ws", HttpContentTypeEnum.application_x_ws)]
    //    Ws,


    //    /// <summary>
    //    /// .ws2
    //    /// </summary>
    //    [FileExtensionEnum(".ws2", HttpContentTypeEnum.application_x_ws)]
    //    Ws2,


    //    /// <summary>
    //    /// .wsc
    //    /// </summary>
    //    [FileExtensionEnum(".wsc", HttpContentTypeEnum.text_scriptlet)]
    //    Wsc,


    //    /// <summary>
    //    /// .wsdl
    //    /// </summary>
    //    [FileExtensionEnum(".wsdl", HttpContentTypeEnum.text_xml)]
    //    Wsdl,


    //    /// <summary>
    //    /// .wvx
    //    /// </summary>
    //    [FileExtensionEnum(".wvx", HttpContentTypeEnum.video_x_ms_wvx)]
    //    Wvx,


    //    /// <summary>
    //    /// .x_b
    //    /// </summary>
    //    [FileExtensionEnum(".x_b", HttpContentTypeEnum.application_x_x_b)]
    //    X_b,


    //    /// <summary>
    //    /// .x_t
    //    /// </summary>
    //    [FileExtensionEnum(".x_t", HttpContentTypeEnum.application_x_x_t)]
    //    X_t,


    //    /// <summary>
    //    /// .xap
    //    /// </summary>
    //    [FileExtensionEnum(".xap", HttpContentTypeEnum.application_x_silverlight_app)]
    //    Xap,


    //    /// <summary>
    //    /// .xdp
    //    /// </summary>
    //    [FileExtensionEnum(".xdp", HttpContentTypeEnum.application_vnd_adobe_xdp)]
    //    Xdp,


    //    /// <summary>
    //    /// .xdr
    //    /// </summary>
    //    [FileExtensionEnum(".xdr", HttpContentTypeEnum.text_xml)]
    //    Xdr,


    //    /// <summary>
    //    /// .xfd
    //    /// </summary>
    //    [FileExtensionEnum(".xfd", HttpContentTypeEnum.application_vnd_adobe_xfd)]
    //    Xfd,


    //    /// <summary>
    //    /// .xfdf
    //    /// </summary>
    //    [FileExtensionEnum(".xfdf", HttpContentTypeEnum.application_vnd_adobe_xfdf)]
    //    Xfdf,


    //    /// <summary>
    //    /// .xhtml
    //    /// </summary>
    //    [FileExtensionEnum(".xhtml", HttpContentTypeEnum.text_html)]
    //    Xhtml,


    //    /// <summary>
    //    /// .xls
    //    /// </summary>
    //    [FileExtensionEnum(".xls", HttpContentTypeEnum.application_vnd_ms_excel)]
    //    Xls,


    //    /// <summary>
    //    /// .xls
    //    /// </summary>
    //    [FileExtensionEnum(".xls", HttpContentTypeEnum.application_x_xls)]
    //    Xls,


    //    /// <summary>
    //    /// .xlw
    //    /// </summary>
    //    [FileExtensionEnum(".xlw", HttpContentTypeEnum.application_x_xlw)]
    //    Xlw,


    //    /// <summary>
    //    /// .xml
    //    /// </summary>
    //    [FileExtensionEnum(".xml", HttpContentTypeEnum.text_xml)]
    //    Xml,


    //    /// <summary>
    //    /// .xpl
    //    /// </summary>
    //    [FileExtensionEnum(".xpl", HttpContentTypeEnum.audio_scpls)]
    //    Xpl,


    //    /// <summary>
    //    /// .xq
    //    /// </summary>
    //    [FileExtensionEnum(".xq", HttpContentTypeEnum.text_xml)]
    //    Xq,


    //    /// <summary>
    //    /// .xql
    //    /// </summary>
    //    [FileExtensionEnum(".xql", HttpContentTypeEnum.text_xml)]
    //    Xql,


    //    /// <summary>
    //    /// .xquery
    //    /// </summary>
    //    [FileExtensionEnum(".xquery", HttpContentTypeEnum.text_xml)]
    //    Xquery,


    //    /// <summary>
    //    /// .xsd
    //    /// </summary>
    //    [FileExtensionEnum(".xsd", HttpContentTypeEnum.text_xml)]
    //    Xsd,


    //    /// <summary>
    //    /// .xsl
    //    /// </summary>
    //    [FileExtensionEnum(".xsl", HttpContentTypeEnum.text_xml)]
    //    Xsl,


    //    /// <summary>
    //    /// .xslt
    //    /// </summary>
    //    [FileExtensionEnum(".xslt", HttpContentTypeEnum.text_xml)]
    //    Xslt,


    //    /// <summary>
    //    /// .xwd
    //    /// </summary>
    //    [FileExtensionEnum(".xwd", HttpContentTypeEnum.application_x_xwd)]
    //    Xwd,





    //}

    #endregion

    #region 安全加密方法中 加密方向类型 枚举

    /// <summary>
    /// 安全加密方法中 加密类型 枚举
    /// </summary>
    public enum SecurityEncryptDirectionTypeEnum
    {

        UnDefine,

        BytesToBytes,
        BytesToFile,

        StringToBytes,
        StringToFile,
        StringToBase64String,

        ModelToBytes,
        ModelToBase64String,
        ModelToFile,

        FileToFile,

        BytesToBitmap,
        StringToBitmap,
        ModelToBitmap,

        BytesToImageFile,
        StringToImageFile,
        ModelToImageFile,


    }


    #endregion




}
