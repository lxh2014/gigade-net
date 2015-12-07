
function editFunction(rowID) {
    //供應商Model
    Ext.define("gigade.Vendor", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "vendor_id", type: "string" },
            { name: "vendor_name_simple", type: "string" }]
    });
    //供應商Store
    var VendorStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Vendor',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/Vendor/GetVendor",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    var row = null;
    var mycount = ShopClassStore.getCount();
    var Shopclass = [];
    for (var i = 1; i < mycount; i++) {
        var boxLabel = ShopClassStore.getAt(i).get("class_name");
        var name = ShopClassStore.getAt(i).get("class_id");
        Shopclass.push({ boxLabel: boxLabel, name: name, inputValue: name });
    };


    function tomorrow() {
        var d;
        var dt;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate();
        dt = new Date(s);
        dt.setDate(dt.getDate() + 1);
        return dt;                                 // 返回日期。
    };
    setNextMonth = function (source, n) {
        var s = new Date(source);
        s.setMonth(s.getMonth() + n);
        if (n < 0) {
            s.setHours(0, 0, 0);
        }
        else if (n > 0) {
            s.setHours(23, 59, 59);
        }
        return s;
    }

    function initForm(row) {
        Ext.getCmp("Image_Name").setRawValue(row.data.Image_Name);
        Ext.getCmp("Resume_Image").setRawValue(row.data.Resume_Image);
        Ext.getCmp("Promotion_Banner_Image").setRawValue(row.data.Promotion_Banner_Image);
        Ext.getCmp("brand_logo").setRawValue(row.data.brand_logo);
        var shopclassid = row.data.classIds;
        var shopclassids = shopclassid.split(",");
        myCheckboxGroup = Ext.getCmp("shopclass").items;
        for (var i = 0; i < shopclassids.length; i++) {
            for (var j = 0; j < myCheckboxGroup.length; j++) {
                if (myCheckboxGroup.items[j].inputValue == shopclassids[i]) {
                    myCheckboxGroup.items[j].setValue(true);
                }
            }
        }
    }
    var firstForm = Ext.widget('form', {
        id: 'editFrm1',
        plain: true,
        frame: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        url: '/Vendor/UpdVendorBrand',
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                var selStartDate = Ext.getCmp("begin_time").getValue();
                var selEndTime = Ext.getCmp("end_time").getValue();
                var checkBoxLength = Ext.getCmp("shopclass").getChecked().length;
                if (selEndTime <= selStartDate) {
                    Ext.Msg.alert(INFORMATION, '開始時間不能大於結束時間');
                    Ext.getCmp("end_time").setValue("");
                    return;
                }
                if (checkBoxLength == 0) {
                    Ext.Msg.alert("提示信息", "請選擇館別");
                    return;
                }

                if (form.isValid()) {
                    var sss = "";
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                    myMask.show();
                    myCheckboxGroup = Ext.getCmp("shopclass").getChecked();
                    for (var i = 0; i < myCheckboxGroup.length; i++) {
                        if (myCheckboxGroup[i].checked) {
                            sss += myCheckboxGroup[i].inputValue + ",";
                        }
                    };
                    this.disable();
                    form.submit({
                        params: {
                            Brand_Id: Ext.htmlEncode(Ext.getCmp('Brand_Id').getValue()),
                            brandName: Ext.htmlEncode(Ext.getCmp('Brand_Name').getValue()),
                            vendorid: Ext.htmlEncode(Ext.getCmp('Vendor_Id').getValue()),
                            shopclass: sss,
                            brandsort: Ext.htmlEncode(Ext.getCmp('Brand_Sort').getValue()),
                            brandstatus: Ext.htmlEncode(Ext.getCmp('Brand_Status').getValue().Status),
                            brandmsg: Ext.htmlEncode(Ext.getCmp('Brand_Msg').getValue()),
                            begin_time: Ext.htmlEncode(Ext.getCmp('begin_time').getValue()),
                            end_time: Ext.htmlEncode(Ext.getCmp('end_time').getValue()),
                            cucumberbrand: Ext.htmlEncode(Ext.getCmp("Cucumber_Brand").getValue().Brand),
                            short_description: Ext.htmlEncode(Ext.getCmp('short_description').getValue()),
                            imagestatus: Ext.htmlEncode(Ext.getCmp("Image_Status").getValue().Hidden),
                            imagelinkmode: Ext.htmlEncode(Ext.getCmp("Image_Link_Mode").getValue().Mode),
                            imagelinkurl: Ext.htmlEncode(Ext.getCmp('Image_Link_Url').getValue()),
                            imagename: Ext.htmlEncode(Ext.getCmp('Image_Name').getValue()),
                            resumeimage: Ext.htmlEncode(Ext.getCmp('Resume_Image').getValue()),
                            resumeimagelink: Ext.htmlEncode(Ext.getCmp('Resume_Image_Link').getValue()),
                            promotionbannerimage: Ext.htmlEncode(Ext.getCmp('Promotion_Banner_Image').getValue()),
                            promotionbannerimagelink: Ext.htmlEncode(Ext.getCmp('Promotion_Banner_Image_Link').getValue()),
                            mediareportlinkurl: Ext.htmlEncode(Ext.getCmp('Media_Report_Link_Url').getValue()),
                            brand_logo: Ext.htmlEncode(Ext.getCmp('brand_logo').getValue()),
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg != "undefined") {
                                    Ext.Msg.alert("提示", result.msg);
                                    VendorBrandStore.load();
                                    editWins.close();
                                } else {
                                    Ext.Msg.alert("提示", result.msg);
                                    VendorBrandStore.load();
                                }
                            } else {
                                Ext.Msg.alert("提示", result.msg);//上傳圖片大小或格式不正確
                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert("提示", "操作失败,請稍后再試,或聯繫開發人員!");
                        }
                    });
                }
            }
        }],
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '品牌編號',
                id: 'Brand_Id',
                name: 'Brand_Id',
                allowBlank: false,
                labelWidth: 90,
                hidden: true
            },
            {//品牌基本信息
                xtype: 'form',
                title: '品牌基本信息',
                id: 'pp',
                hidden: false,
                layout: 'anchor',
                border: 0,
                frame: true,
                allowBlank: false,
                defaults: {
                    anchor: '100%'
                },
                items: [
                {
                    xtype: 'textfield',
                    fieldLabel: '品牌名稱<font color="red">*</font>',
                    id: 'Brand_Name',
                    name: 'Brand_Name',
                    allowBlank: false,
                    hidden: false,
                    anchor: '90%'
                },
                {
                    xtype: 'combobox',
                    fieldLabel: '供應商簡稱<font color="red">*</font>',
                    allowBlank: false,
                    editable: true,
                    queryMode: 'local',
                    hidden: false,
                    id: 'Vendor_Id',
                    name: 'Vendor_Id',
                    store: VendorStore,
                    displayField: 'vendor_name_simple',
                    valueField: 'vendor_id',
                    typeAhead: true,
                    forceSelection: false,
                    lastQuery: '',
                    anchor: '90%',
                    emptyText: 'SELECT'
                },
                {
                    xtype: 'checkboxgroup',
                    fieldLabel: '館別<font color="red">*</font>',
                    id: 'shopclass',
                    name: 'shopclass',
                    columns: 5,
                    items: Shopclass
                },
                {
                    xtype: 'numberfield',
                    fieldLabel: '排序',
                    id: 'Brand_Sort',
                    name: 'Brand_Sort',
                    maxValue: 65535,
                    minValue: 0,
                    allowBlank: false,
                    hidden: false,
                    anchor: '90%'
                },
                {
                    xtype: 'radiogroup',
                    fieldLabel: '狀態',
                    hidden: false,
                    id: 'Brand_Status',
                    name: 'Brand_Status',
                    colName: 'brand_status',
                    defaults: {
                        name: 'Status',
                        margin: '0 8 0 0'
                    },
                    columns: 2,
                    vertical: true,
                    items: [
                    { boxLabel: '顯示', id: 'active', inputValue: '1', checked: true },
                    { boxLabel: '隱藏', id: 'nonactive', inputValue: '2' }
                    ]
                },
                {
                    xtype: 'textfield',
                    fieldLabel: '廣告(20字內)',
                    id: 'Brand_Msg',
                    name: 'Brand_Msg',
                    maxLength: 20,
                    anchor: '90%'
                },
                {
                    xtype: "datetimefield",
                    fieldLabel: '顯示開始時間',
                    id: 'begin_time',
                    name: 'begin_time',
                    format: 'Y-m-d H:i:s',
                    time: { hour: 00, min: 00, sec: 00 },
                    editable: false,
                    anchor: '90%',
                    submitValue: true,
                    enable: false,
                    editable: false,
                    value: Tomorrow(),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("begin_time");
                            var end = Ext.getCmp("end_time");
                            if (start.getValue() > end.getValue()) {
                                Ext.Msg.alert(INFORMATION, '開始時間不能大於結束時間');
                                end.setValue(setNextMonth(start.getValue(), 1));
                            }
                        }
                    }
                },
                {
                    xtype: "datetimefield",
                    id: 'end_time',
                    name: 'end_time',
                    format: 'Y-m-d H:i:s',
                    time: { hour: 23, min: 59, sec: 59 },
                    editable: false,
                    fieldLabel: '顯示結束時間',
                    anchor: '90%',
                    submitValue: true,
                    editable: false,
                    value: setNextMonth(Tomorrow(), 1),
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("begin_time");
                            var end = Ext.getCmp("end_time");
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, '結束時間不能小於開始時間');
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                        }
                    }
                },
                {
                    xtype: 'radiogroup',
                    fieldLabel: '品牌列表',
                    hidden: false,
                    id: 'Cucumber_Brand',
                    name: 'Cucumber_Brand',
                    colName: 'cucumber_brand',
                    anchor: '90%',
                    defaults: {
                        name: 'Brand',
                        margin: '0 8 0 0'
                    },
                    columns: 4,
                    vertical: true,
                    items: [
                    { boxLabel: '吉甲地品牌', id: 'cb', inputValue: '0', checked: true },
                    { boxLabel: '廣源良品牌', id: 'cb1', inputValue: '1' },
                    { boxLabel: '大陸區品牌', id: 'cb2', inputValue: '2' },
                    { boxLabel: '小農品牌 ', id: 'cb3', inputValue: '3' }
                    ]
                },
                 {
                     xtype: 'textareafield',
                     fieldLabel: '短文字說明 (300字內)',
                     id: 'short_description',
                     name: 'short_description',
                     anchor: '90%',
                     maxLength: 300
                 }
                ]
            },
            {//形象圖管理
                xtype: 'form',
                title: '形象圖管理',
                id: 'xxt',
                hidden: false,
                layout: 'anchor',
                border: 0,
                frame: true,
                allowBlank: false,
                defaults: {
                    anchor: '100%'
                },
                items: [
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '是否顯示',
                        hidden: false,
                        id: 'Image_Status',
                        name: 'Image_Status',
                        colName: 'vendor_status',
                        defaults: {
                            name: 'Hidden',
                            margin: '0 8 0 0'
                        },
                        columns: 2,
                        vertical: true,
                        items: [
                        { boxLabel: '顯示', id: 'vh1', inputValue: '1', checked: true },
                        { boxLabel: '隱藏', id: 'vh2', inputValue: '0' }
                        ]
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '連接模式',
                        hidden: false,
                        id: 'Image_Link_Mode',
                        name: 'Image_Link_Mode',
                        colName: 'image_link_mode',
                        defaults: {
                            name: 'Mode',
                            margin: '0 8 0 0'
                        },
                        columns: 2,
                        vertical: true,
                        items: [
                        { boxLabel: '母視窗連接', id: 'cn1', inputValue: '1', checked: true },
                        { boxLabel: '新視窗開啟', id: 'cn2', inputValue: '0' }
                        ]
                    },
                     {
                         xtype: 'fieldcontainer',
                         combineErrors: true,
                         layout: 'hbox',
                         anchor: '90%',
                         items: [
                     {
                         xtype: 'filefield',
                         fieldLabel: '形象圖片',
                         name: 'Image_Name',
                         id: 'Image_Name',
                         msgTarget: 'side',
                         allowBlank: true,
                         flex: 1,
                         buttonText: '選擇...',
                         fileUpload: true
                     },
                      {
                          xtype: 'button', id: 'delImage', margin: '0 0 0 3', iconCls: 'icon-cross', handler: onDelProPicClick
                      }]
                     },
                    {
                        xtype: 'textfield',
                        fieldLabel: '圖片連接地址',
                        id: 'Image_Link_Url',
                        name: 'Image_Link_Url',
                        vtype: 'url',
                        allowBlank: true,
                        hidden: false,
                        anchor: '90%'
                    },
                     {
                         xtype: 'fieldcontainer',
                         combineErrors: true,
                         layout: 'hbox',
                         anchor: '90%',
                         items: [
                    {
                        xtype: 'filefield',
                        fieldLabel: '安心聲明圖片',
                        name: 'Resume_Image',
                        id: 'Resume_Image',
                        msgTarget: 'side',
                        allowBlank: true,
                        flex: 1,
                        buttonText: '選擇...',
                        fileUpload: true
                    },
                     {
                         xtype: 'button', id: 'delResume', margin: '0 0 0 3', iconCls: 'icon-cross', handler: onDelProPicClick
                     }]
                     },
                    {
                        xtype: 'textfield',
                        fieldLabel: '圖片連接地址',
                        id: 'Resume_Image_Link',
                        name: 'Resume_Image_Link',
                        vtype: 'url',
                        allowBlank: true,
                        hidden: false,
                        anchor: '90%'
                    },
                     {
                         xtype: 'fieldcontainer',
                         combineErrors: true,
                         layout: 'hbox',
                         anchor: '90%',
                         items: [
                    {
                        xtype: 'filefield',
                        fieldLabel: '促銷圖片',
                        id: 'Promotion_Banner_Image',
                        name: 'Promotion_Banner_Image',
                        msgTarget: 'side',
                        allowBlank: true,
                        flex: 1,
                        buttonText: '選擇...',
                        fileUpload: true
                    },
                    {
                        xtype: 'button', id: 'delPromo', margin: '0 0 0 3', iconCls: 'icon-cross', handler: onDelProPicClick
                    }]
                     },
                    {
                        xtype: 'textfield',
                        fieldLabel: '圖片連接地址',
                        id: 'Promotion_Banner_Image_Link',
                        name: 'Promotion_Banner_Image_Link',
                        vtype: 'url',
                        allowBlank: true,
                        hidden: false,
                        anchor: '90%'
                    },
                                                             {
                                                                 xtype: 'fieldcontainer',
                                                                 combineErrors: true,
                                                                 layout: 'hbox',
                                                                 anchor: '90%',
                                                                 items: [
                                                            {
                                                                xtype: 'filefield',
                                                                fieldLabel: '品牌logo',
                                                                id: 'brand_logo',
                                                                name: 'brand_logo',
                                                                msgTarget: 'side',
                                                                allowBlank: true,
                                                                flex: 1,
                                                                buttonText: '選擇...',
                                                                fileUpload: true
                                                            },
                                                            {
                                                                xtype: 'button', id: 'delBrandLogo', margin: '0 0 0 3', iconCls: 'icon-cross', handler: onDelProPicClick
                                                            }]
                                                             },
                ]
            },
            {//媒體報道
                xtype: 'form',
                hidden: false,
                layout: 'anchor',
                border: 0,
                frame: true,
                title: '媒體報道',
                id: 'mt',
                allowBlank: false,
                defaults: {
                    anchor: '100%'
                },
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'YouTube網址',
                        id: 'Media_Report_Link_Url',
                        name: 'Media_Report_Link_Url',
                        allowBlank: true,
                        hidden: false,
                        anchor: '90%'
                    },
                    {
                        xtype: 'displayfield',
                        id: 'dis',
                        value: '<span style="color:gray">*</span><a id="yTE" href="#" >YouTube嵌入程序碼取得說明</a>',
                        width: 200,
                        listeners: {
                            afterrender: function () {
                                Ext.create("Ext.tip.ToolTip", {
                                    target: "dis",
                                    maxWidth: 500,
                                    width: 460,
                                    height: 250,
                                    html: '<img src="../../Content/img/youtube_url_link_step.jpg"/>'
                                });
                            }
                        }
                    }
                ]
            }
        ]
    });
    var editWins = Ext.create('Ext.window.Window', {
        title: '品牌列表',
        id: 'editWins',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        width: 510,
        height: 510,
        layout: 'fit',
        items: firstForm,
        constrain: true, //束縛
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: '是否關閉',
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (row) {
                    firstForm.getForm().loadRecord(row);
                    //                    var event = row.data.Event.toString();
                    //                    if (event == "1") {
                    //                        Ext.getCmp('show').setValue(true);
                    //                        Ext.getCmp('hidden').setValue(false);
                    //                    }
                    //                    else {
                    //                        Ext.getCmp('hidden').setValue(true);
                    //                        Ext.getCmp('show').setValue(false);
                    //                    }
                    initForm(row);
                } else {
                    firstForm.getForm().reset();
                }
                switch (row.data.Cucumber_Brand) {
                    case "0":
                        Ext.getCmp('cb').setValue(true);
                        break;
                    case "1":
                        Ext.getCmp('cb1').setValue(true);
                        break;
                    case "2":
                        Ext.getCmp('cb2').setValue(true);
                        break;
                    case "3":
                        Ext.getCmp('cb3').setValue(true);
                        break;
                }
                if (row.data.Image_Status == "0") {
                    Ext.getCmp('vh2').setValue(true);
                } else {
                    Ext.getCmp('vh1').setValue(true);
                }
                if (row.data.Image_Link_Mode == "1") {
                    Ext.getCmp('cn1').setValue(true);
                } else {
                    Ext.getCmp('cn2').setValue(true);
                }
                if (row.data.Brand_Status == "1") {
                    Ext.getCmp('active').setValue(true);
                } else {
                    Ext.getCmp('nonactive').setValue(true);
                }
            }
        }
    });
    //editWins.show();

    if (rowID !== null) {
        edit_VendorBrandStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_VendorBrandStore.getAt(0);
                VendorStore.load({
                    callback: function () {
                        editWins.show();
                    }
                })
            }
        });

    }
    else {
        editWins.show();
    }


    function onDelProPicClick() {
        var type = "";
        var targetID = "";
        switch (this.id) {
            case "delImage":
                type = "image_name";
                targetID = "Image_Name";
                break;
            case "delResume":
                type = "resume_image";
                targetID = "Resume_Image";
                break;
            case "delPromo":
                type = "promotion_banner_image";
                targetID = "Promotion_Banner_Image";
                break;
            case "delBrandLogo":
                type = "brand_logo";
                targetID = "brand_logo";
                break;
            default:
                break;

        }
        var pic = Ext.getCmp(targetID).getValue();
        if (pic != null && pic != "" && type != "") {
            Ext.MessageBox.confirm(CONFIRM, "圖片刪除后將無法找回,是否確認刪除？", function (btn) {
                if (btn === "yes") {
                    Ext.Ajax.request({
                        url: '/Vendor/DelPromoPicClick',
                        method: 'post',
                        params: {
                            "brand_id": row.data.Brand_Id,
                            "type": type,
                            "src": pic
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {

                                Ext.getCmp(targetID).setRawValue("");
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "圖片刪除失敗！");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "圖片刪除失敗！");
                        }

                    });
                    VendorBrandStore.load();
                }

            });

        }
    }
}

