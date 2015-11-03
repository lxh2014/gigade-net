

function addFunction(RowID, VendorBrandsetaddStore) {
    var mycount = ShopClassStore.getCount();
    Shopclass = [];
    for (var i = 1; i < mycount; i++) {
        var boxLabel = ShopClassStore.getAt(i).get("class_name");
        var name = ShopClassStore.getAt(i).get("class_id");
        Shopclass.push({ boxLabel: boxLabel, name: name, inputValue: name });
    };
    var VendorID = "";
    var BrandID = "";
    if (RowID != null) {
        VendorID = RowID.data.Vendor_Id;
    }
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
        autoLoad: true,
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
    var firstForm = Ext.widget('form',
    {
        id: 'editFrm1',
        plain: true,
        frame: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        url: '/Vendor/SaveVendorBrand',
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                var selStartDate = Ext.getCmp("Brand_Msg_Start_Time").getValue();
                var selEndTime = Ext.getCmp("Brand_Msg_End_Time").getValue();
                if (selEndTime <= selStartDate) {
                    Ext.Msg.alert(INFORMATION, TIMETIP);
                    Ext.getCmp("Brand_Msg_End_Time").setValue("");
                    return;
                }

                if (form.isValid()) {
                    var sss = "";
                    myCheckboxGroup = Ext.getCmp("shopclass").getChecked();
                    for (var i = 0; i < myCheckboxGroup.length; i++) {
                        if (myCheckboxGroup[i].checked) {
                            sss += myCheckboxGroup[i].inputValue + ",";
                        }
                    };
                    form.submit({
                        params: {
                            brandName: Ext.htmlEncode(Ext.getCmp('Brand_Name').getValue()),
                            vendorid: Ext.htmlEncode(Ext.getCmp('Vendor_Id').getValue()),
                            shopclass: sss,
                            brandsort: Ext.htmlEncode(Ext.getCmp('Brand_Sort').getValue()),
                            brandstatus: Ext.htmlEncode(Ext.getCmp('Brand_Status').getValue().Status),
                            brandmsg: Ext.htmlEncode(Ext.getCmp('Brand_Msg').getValue()),
                            Brand_Msg_Start_Time: Ext.htmlEncode(Ext.getCmp('Brand_Msg_Start_Time').getValue()),
                            Brand_Msg_End_Time: Ext.htmlEncode(Ext.getCmp('Brand_Msg_End_Time').getValue()),
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
                            mediareportlinkurl: Ext.htmlEncode(Ext.getCmp('Media_Report_Link_Url').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg != "undefined") {
                                    Ext.Msg.alert("提示", result.msg);
                                    VendorBrandStore.load();
                                    editWin.close();
                                } else {
                                    Ext.Msg.alert("提示", result.msg);
                                    VendorBrandStore.load();
                                    editWin.close();
                                }
                            } else {
                                Ext.Msg.alert("提示", result.msg);
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert("提示", "操作失败,請稍后再試,或聯繫開發人員!");
                        }
                    });
                }
            }
        }],
        items: [
            {//品牌基本信息
                xtype: 'form',
                hidden: false,
                layout: 'anchor',
                border: 0,
                frame: true,
                title: '品牌基本信息',
                id: 'pp',
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
                        anchor: '90%',
                        emptyText: 'SELECT'
                    },
                    {
                        xtype: 'checkboxgroup',
                        fieldLabel: '館別<font color="red">*</font>',
                        allowBlank: false,
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
                        allowBlank: false,
                        hidden: false,
                        maxValue: 65535,
                        minValue: 0,
                        value: 0,
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
                        //                        allowBlank: false,
                        anchor: '90%'
                    },
                    {
                        xtype: "datetimefield",
                        fieldLabel: '顯示開始時間',
                        id: 'Brand_Msg_Start_Time',
                        name: 'Brand_Msg_Start_Time',
                        format: 'Y-m-d H:i:s',
                        anchor: '80%',
                        submitValue: true,
                        enable: false,
                        value: Tomorrow(),
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("Brand_Msg_Start_Time");
                                var end = Ext.getCmp("Brand_Msg_End_Time");
                                var s_date = new Date(start.getValue());
                                end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));
                            }
                        }
                    },
                    {
                        xtype: "datetimefield",
                        id: 'Brand_Msg_End_Time',
                        name: 'Brand_Msg_End_Time',
                        format: 'Y-m-d H:i:s',
                        fieldLabel: '顯示結束時間',
                        anchor: '80%',
                        submitValue: true,
                        value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("Brand_Msg_Start_Time");
                                var end = Ext.getCmp("Brand_Msg_End_Time");
                                if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, '開始時間不能大於結束時間');
                                    end.setValue(new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)));
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
                        defaults: {
                            name: 'Brand'
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
                hidden: false,
                layout: 'anchor',
                border: 0,
                frame: true,
                title: '形象圖管理',
                id: 'xxt',
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
                    xtype: 'filefield',
                    fieldLabel: '形象圖片',
                    name: 'Image_Name',
                    id: 'Image_Name',
                    msgTarget: 'side',
                    allowBlank: true,
                    submitValue: true,
                    anchor: '90%',
                    buttonText: '選擇...',
                    fileUpload: true
                },
                 {
                     xtype: 'textfield',
                     fieldLabel: '圖片連接地址',
                     id: 'Image_Link_Url',
                     vtype: 'url',
                     name: 'Image_Link_Url',
                     hidden: false,
                     anchor: '90%'
                 },
                {
                    xtype: 'filefield',
                    fieldLabel: '安心聲明圖片',
                    name: 'Resume_Image',
                    id: 'Resume_Image',
                    msgTarget: 'side',
                    allowBlank: true,
                    submitValue: true,
                    anchor: '90%',
                    buttonText: '選擇...',
                    fileUpload: true
                },
                {
                    xtype: 'textfield',
                    fieldLabel: '圖片連接地址',
                    id: 'Resume_Image_Link',
                    name: 'Resume_Image_Link',
                    vtype: 'url',
                    hidden: false,
                    anchor: '90%'
                },
                {
                    xtype: 'filefield',
                    fieldLabel: '促銷圖片',
                    id: 'Promotion_Banner_Image',
                    name: 'Promotion_Banner_Image',
                    msgTarget: 'side',
                    submitValue: true,
                    allowBlank: true,
                    anchor: '90%',
                    buttonText: '選擇...',
                    fileUpload: true
                },
                {
                    xtype: 'textfield',
                    fieldLabel: '圖片連接地址',
                    id: 'Promotion_Banner_Image_Link',
                    name: 'Promotion_Banner_Image_Link',
                    vtype: 'url',
                    hidden: false,
                    anchor: '90%'
                }
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
                defaults: {
                    anchor: '100%'
                },
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'YouTube網址',
                        id: 'Media_Report_Link_Url',
                        name: 'Media_Report_Link_Url',
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
    function tomorrow() {
        var d;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate();                          // 获取日。
        return (new Date(s));                                 // 返回日期。
    };
    var editWin = Ext.create('Ext.window.Window', {
        title: '品牌列表',
        id: 'editWin',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 510,
        height: 510,
        layout: 'fit',
        items: firstForm,
        constrain: true,
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
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],

        listeners: {
            'show': function () {
                if (RowID) {
                    firstForm.getForm().loadRecord(RowID);
                }
                else {
                    firstForm.getForm().reset();
                }
            }
        }
    });
    VendorStore.load({
        callback: function () {
            editWin.show();
        }
    })
    function imgFadeBig(img, width, height) {
        var e = this.event;
        if (img.split('/').length != 5) {
            $("#imgTip").attr("src", img)
            .css({
                "top": (e.clientY < height ? e.clientY : e.clientY - height) + "px",
                "left": (e.clientX) + "px",
                "width": width + "px",
                "height": height + "px"
            }).show();
        }

    }

}
