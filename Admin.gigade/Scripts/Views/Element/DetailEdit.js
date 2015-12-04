
function selectionChange() {
    var lis = $('.k-selectable').children();
    for (var i = 0; i < lis.length; i++) {
        var current = $(lis[i]);
        var selected = current.attr("aria-selected");
        if (selected == "true") {
            var dType = current.attr("data-type");
            if (dType == "d") {
                url.val('http://');
                return;
            }
            else if (dType == "f") {
                url.val(decodeURIComponent(url.val()));
                break;
            }
        }
    }
}
function editFunction(RowID, Store, i) {
    var treeCateStore = Ext.create('Ext.data.TreeStore', {
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/Product/GetProductCatagory',
            actionMethods: 'post'
        },
        rootVisible: false,
        root: {
            text: '商品類別',
            expanded: true,
            children: []
        }
    });
    treeCateStore.load();

    //area_packet
    Ext.define("gigade.Packet", {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'packet_id', type: 'string' },
            { name: 'packet_name', type: 'string' }
        ]
    });

    var PacketStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Packet',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Element/GetPacket",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }

    });


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Element/SaveElementDetaiil',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },

        //////編輯器
        listeners: {
            'afterrender': function () {
                jQuery(function () {
                    jQuery('textarea[name=kendoEditor]').kendoEditor({
                        "tools": [
                            { "name": "bold" },
                            { "name": "italic" },
                            { "name": "underline" },
                            { "name": "strikethrough" },
                            { "name": "justifyLeft" },
                            { "name": "justifyCenter" },
                            { "name": "justifyRight" },
                            { "name": "justifyFull" },
                            { "name": "insertUnorderedList" },
                            { "name": "insertOrderedList" },
                            { "name": "outdent" },
                            { "name": "indent" },
                            {
                                "name": "cLink",
                                "tooltip": KENDOLINK,
                                "exec":
                                    function (e) {
                                        var editor = $(this).data("kendoEditor");
                                        editor.exec("createLink");
                                        $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                        $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可                                      
                                    }

                            },
                            { "name": "unlink" },
                            {
                                "name": "iFile",
                                "tooltip": KENDOFILE,
                                "exec":
                                   function (e) {
                                       var editor = $(this).data("kendoEditor");
                                       editor.exec("insertFile");
                                       $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                       $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可                                     
                                   }
                            },
                            {
                                "name": "iImage",
                                "tooltip": INSERTIMG,
                                "exec":
                                    function (e) {
                                        var editor = $(this).data("kendoEditor");
                                        editor.exec("insertImage");
                                        url = $('#k-editor-image-url');
                                        $('.k-selectable').attr("onclick", "selectionChange()");
                                        $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                        $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可
                                        $('.k-window').css('top', 20);
                                        $('.k-window').css('height', 340);
                                        $('.k-window').css('width', 700);
                                    }
                            },
                            { "name": "subscript" },
                            { "name": "superscript" },
                            { "name": "createTable" },
                            { "name": "addColumnLeft" },
                            { "name": "addColumnRight" },
                            { "name": "addRowAbove" },
                            { "name": "addRowBelow" },
                            { "name": "deleteRow" },
                            { "name": "deleteColumn" },
                            {
                                "name": "vHtml",
                                "tooltip": KENDOHTML,
                                "exec":
                               function (e) {
                                   var editor = $(this).data("kendoEditor");
                                   editor.exec("viewHtml");
                                   $('.k-overlay').css('z-index', 19012);//设置遮罩zindex
                                   $('.k-window').css('z-index', 19013);//设置文件选择框zindex,比上面遮罩大1即可
                               }
                            },
                            { "name": "formatting" },
                            { "name": "fontName" },
                            { "name": "fontSize" },
                            { "name": "foreColor" },
                            { "name": "backColor" }
                        ],
                        "imageBrowser": {
                            "transport": {
                                "read": { "url": "/ImageBrowser/Read" },
                                "type": "imagebrowser-aspnetmvc",
                                "thumbnailUrl": "/ImageBrowser/Thumbnail",
                                "uploadUrl": "/ImageBrowser/Upload",
                                "destroy": { "url": "/ImageBrowser/Destroy" },
                                "create": { "url": "/ImageBrowser/Create" },
                                //  "imageUrl": "http://192.168.16.118/uploads/{0}"
                                "imageUrl": document.getElementById('BaseAddress').value + "/" + "" + document.getElementById('path').value + "" + "/" + "{0}"
                            }
                        }
                    });
                });
            }

        },
        /////編輯器
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'ID',
                id: 'element_id',
                name: 'element_id',
                hidden: true
            },
            {
                xtype: 'combobox',
                store: PacketStore,
                fieldLabel: PACKET,
                lastQuery: '',
                id: 'packet_id',
                name: 'packet_id',
                displayField: 'packet_name',
                valueField: 'packet_id',
                allowBlank: false,
                editable: false,
                emptyText: SELECT,
                listeners: {
                    select:
                        function (combo, records, eOpts) {
                            Ext.Ajax.request({
                                url: '/Element/GetId',
                                method: 'post',
                                async: true,
                                params: {
                                    PacketId: Ext.getCmp("packet_id").getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) { }
                                    else
                                    {
                                        Ext.Msg.alert(INFORMATION, PACKETNUMBER);
                                        // Ext.getCmp('editWin').destroy();
                                        Ext.getCmp('packet_id').setValue("");
                                    }
                                }
                            });
                            //獲取packet中的element_type
                            Ext.Ajax.request({
                                url: '/Element/GetElementTypeInPacket',
                                method: 'post',
                                async: false,
                                params: { PacketId: Ext.getCmp("packet_id").getValue() },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        switch (result.elementType) {
                                            case 1:
                                                Ext.getCmp("isImage").setValue(true);
                                                Ext.getCmp("isContent").setValue(false);
                                                Ext.getCmp("isProduct").setValue(false);
                                                Ext.getCmp("element_type").setReadOnly(true);
                                                break;
                                            case 2:
                                                Ext.getCmp("isContent").setValue(true);
                                                Ext.getCmp("isImage").setValue(false);
                                                Ext.getCmp("isProduct").setValue(false);
                                                Ext.getCmp("element_type").setReadOnly(true);
                                                break;
                                            case 3:
                                                Ext.getCmp("isProduct").setValue(true);
                                                Ext.getCmp("isImage").setValue(false);
                                                Ext.getCmp("isContent").setValue(false);
                                                Ext.getCmp("element_type").setReadOnly(true);
                                                break;
                                            default:
                                                Ext.Msg.alert(INFORMATION, GETPACKETINFOERROR);
                                                break;
                                        }
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, GETPACKETINFOERROR);
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, GETPACKETINFOERROR);
                                }
                            });

                        }
                }
            },
            {
                xtype: 'radiogroup',//元素類型 1：圖片 2：文字
                hidden: false,
                id: 'element_type',
                name: 'element_type',
                fieldLabel: ELEMENTTYPE,
                colName: 'element_type',
                width: 400,
                defaults: {
                    name: 'element_type',
                    margin: '0 8 0 0'
                },
                columns: 3,
                vertical: true,
                items: [
                    {
                        boxLabel: IMAGE, id: 'isImage', inputValue: '1', checked: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var targetImage = Ext.getCmp("element_image");
                                var targetImage2 = Ext.getCmp("element_img_big");
                                var delbig = Ext.getCmp("delbig");
                                var targetContent = Ext.getCmp("kendoEditor");
                                var productidset = Ext.getCmp("productidset");
                                var imgcateset = Ext.getCmp("imgcateset");
                                var targetProduct = Ext.getCmp("product_id");
                                var category_name = Ext.getCmp("category_name");
                                var category_id = Ext.getCmp("category_id");
                                if (newValue) {
                                    targetImage.allowBlank = false;
                                    targetContent.allowBlank = true;
                                    targetProduct.allowBlank = true;
                                    targetImage.setRawValue("");
                                    targetImage2.setRawValue("");
                                    targetContent.setValue("");
                                    targetProduct.setValue("");
                                    category_name.setValue("");
                                    category_id.setValue("");

                                    targetImage.show();
                                    targetImage2.show();
                                    delbig.show();
                                    imgcateset.show();
                                    targetContent.hide();
                                    productidset.hide();
                                    if (category_name.getValue() != "") {
                                        category_name.show();
                                    } else {
                                        category_name.hide();
                                    }
                                }
                            }
                        }
                    },
                    {
                        boxLabel: TEXT, id: 'isContent', inputValue: '2',
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var targetImage = Ext.getCmp("element_image");
                                var targetImage2 = Ext.getCmp("element_img_big");
                                var delbig = Ext.getCmp("delbig");
                                var targetContent = Ext.getCmp("kendoEditor");
                                var targetProduct = Ext.getCmp("product_id");
                                var productidset = Ext.getCmp("productidset");
                                var imgcateset = Ext.getCmp("imgcateset");
                                var category_name = Ext.getCmp("category_name");
                                var category_id = Ext.getCmp("category_id");
                                if (newValue) {
                                    targetImage.allowBlank = true;
                                    targetContent.allowBlank = true;
                                    targetProduct.allowBlank = true;
                                    targetImage.setRawValue("");
                                    targetImage2.setRawValue("");
                                    targetContent.setValue("");
                                    targetProduct.setValue("");
                                    category_name.setValue("");
                                    category_id.setValue("");

                                    targetImage.hide();
                                    targetImage2.hide("");
                                    delbig.hide("");
                                    targetContent.show();
                                    productidset.hide();
                                    category_name.hide();
                                    imgcateset.hide();
                                    category_id.hide();
                                }
                            }
                        }
                    },
                    {
                        boxLabel: PROD, id: 'isProduct', inputValue: '3',
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var targetImage = Ext.getCmp("element_image");
                                var targetImage2 = Ext.getCmp("element_img_big");
                                var delbig = Ext.getCmp("delbig");
                                var targetContent = Ext.getCmp("kendoEditor");
                                var productidset = Ext.getCmp("productidset");
                                var imgcateset = Ext.getCmp("imgcateset");
                                var targetProduct = Ext.getCmp("product_id");
                                var category_name = Ext.getCmp("category_name");
                                var category_id = Ext.getCmp("category_id");
                                if (newValue) {
                                    targetImage.allowBlank = true;
                                    targetContent.allowBlank = true;
                                    targetProduct.allowBlank = false;
                                    targetImage.setRawValue("");
                                    targetImage2.setRawValue("");
                                    targetContent.setValue("");
                                    targetProduct.setValue("");
                                    category_name.setValue("");
                                    category_id.setValue("");

                                    targetImage.hide();
                                    targetImage2.show();
                                    delbig.show();
                                    targetContent.hide();
                                    imgcateset.hide();
                                    productidset.show();
                                    if (category_name.getValue() != "") {
                                        category_name.show();
                                    } else {
                                        category_name.hide();
                                    }
                                }
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                defaults: {
                    labelWidth: 80
                },
                id: 'productidset',
                hidden: true,
                combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: 'numberfield',
                    fieldLabel: PROID,
                    allowBlank: true,
                    hidden: false,
                    readOnly: true,
                    id: 'product_id',
                    name: 'product_id',
                    width: 200,
                    minValue: 0
                },
                {//商品設定
                    xtype: 'button',
                    text: SELECTPRO,
                    allowBlank: true,
                    hidden: false,
                    id: 'productid',
                    name: 'productid',
                    width: 80,
                    minValue: 0,
                    margin: '0 10 0 10',
                    handler: function () {
                        var packetid = Ext.getCmp("packet_id").getValue();
                        var packetname = Ext.getCmp("packet_id").getRawValue();
                        if (Ext.getCmp("element_id").getValue == "") {
                            ElementProductShow(packetid, packetname, '', '');
                        }
                        else {
                            var categoryid = Ext.getCmp("category_id").getValue();
                            var selectproductid = Ext.getCmp("product_id").getValue();
                            ElementProductShow(packetid, packetname, categoryid, selectproductid);
                        }
                    }
                }
                ]
            },
            {
                xtype: 'textfield',
                fieldLabel: ELEMENTTITLE,
                allowBlank: false,
                id: 'element_name',
                name: 'element_name',
                maxLength: "50"
            },
            {
                xtype: 'fieldcontainer',
                defaults: {
                    labelWidth: 80
                },
                id: 'imgcateset',
                hidden: false,
                //  combineErrors: true,
                layout: 'hbox',
                items: [
                {
                    xtype: 'filefield',
                    name: 'element_image',
                    id: 'element_image',
                    fieldLabel: ELEIMG,
                    hidden: false,
                    msgTarget: 'side',
                    width: 500,
                    buttonText: TAKEALOOK,
                    submitValue: true,
                    allowBlank: false,
                    fileUpload: true
                },
                {
                    xtype: 'button',
                    text: SELECTIMGCATE,
                    hidden: false,
                    id: 'imgcateid',
                    name: 'imgcateid',
                    width: 100,
                    minValue: 0,
                    margin: '0 10 0 10',
                    handler: function () {
                        var categoryid = Ext.getCmp("category_id").getValue();
                        var categoryname = Ext.getCmp("category_name").getValue();
                        SelectImgCate(categoryid, categoryname, treeCateStore);
                    }
                }
                ]
            },            
            {
                xtype: 'fieldcontainer',
                defaults: {
                    labelWidth: 80
                },
                id: 'imgcateset1',
                hidden: false,
                //  combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'filefield',
                        name: 'element_img_big',
                        id: 'element_img_big',
                        fieldLabel: "元素圖(大)",
                        hidden: false,
                        msgTarget: 'side',
                        width: 500,
                        buttonText: TAKEALOOK,
                        submitValue: true,
                        fileUpload: true
                    },
                     {
                         xtype: 'button', id: 'delbig', margin: '0 0 0 10', iconCls: 'icon-cross',
                         handler: function () {
                             var targetImage2 = Ext.getCmp("element_img_big");
                             targetImage2.setRawValue("");
                         }
                     }
                    // ,
                    //{
                    //    xtype: 'button',
                    //    text: '刪除',
                    //    hidden: false,
                    //    id: 'delbig',
                    //    name: 'delbig',
                    //    width: 100,
                    //    minValue: 0,
                    //    margin: '0 10 0 10',
                    //    handler: function () {
                    //        var targetImage2 = Ext.getCmp("element_img_big");
                    //        targetImage2.setRawValue("");
                    //    }
                    //}
                ]
            },
            {//類別id
                xtype: 'textfield',
                fieldLabel: CATEID,
                editable: false,
                hidden: true,
                id: 'category_id',
                name: 'category_id',
                minValue: 0
            },
            {//類別名稱
                xtype: 'displayfield',
                fieldLabel: CATENAME,
                hidden: true,
                editable: false,
                id: 'category_name',
                name: 'category_name'
            },
            {
                xtype: 'textareafield',
                fieldLabel: ELECONTENT,
                allowBlank: true,
                hidden: true,
                id: 'kendoEditor',
                name: 'kendoEditor'
                // maxLength: "200"
            },
            {//元素鏈接地址
                xtype: 'textfield',
                fieldLabel: LINKURL,
                allowBlank: true,
                vtype: 'url',
                value: i,
                id: 'element_link_url',
                name: 'element_link_url'
            },
            {
                xtype: 'combobox', //開新視窗 0:不開 1:開原 2：開新
                allowBlank: false,
                editable: false,
                typeAhead: true,
                forceSelection: false,
                fieldLabel: LINKMODE,
                id: 'element_link_mode',
                name: 'element_link_mode',
                hiddenName: 'element_link_mode',
                colName: 'element_link_mode',
                store: linkModelStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                lastQuery: '',
                value: 1
            },
            {
                xtype: 'numberfield',
                name: 'element_sort',
                id: 'element_sort',
                fieldLabel: '排序',
                allowDecimals:false,
                minValue: 0,
                maxValue:9999
            },
            {
                xtype: "datetimefield",
                fieldLabel: BANNERSTSRT,
                editable: false,
                id: 'element_start',
                name: 'element_start',
                format: 'Y-m-d H:i:s',
                allowBlank: false,
                submitValue: true,
                time: { hour: 00, min: 00, sec: 00 },
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("element_start");
                        var end = Ext.getCmp("element_end");
                        if (start.getValue() > end.getValue() && end.getValue() != null) {
                            Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                            end.setValue("");
                        } 
                    }
                }
            },
            {
                xtype: "datetimefield",
                fieldLabel: BANNEREND,
                editable: false,
                id: 'element_end',
                name: 'element_end',
                format: 'Y-m-d H:i:s',
                allowBlank: false,
                submitValue: true,
                time: { hour: 23, min: 59, sec: 59 },
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("element_start");
                        var end = Ext.getCmp("element_end");
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                            end.setValue("");
                        }
                    }
                }
            },
            {
                xtype: 'textarea',
                fieldLabel: ESCINFO,
                id: 'element_remark',
                name: 'element_remark'
            }
        ],
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                if (form.isValid()) {
                    if (Ext.getCmp("element_type").getValue().element_type == 2 && Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()) == "") {
                        Ext.Msg.alert(INFORMATION, KENDOTIP);
                        return;
                    }
                    if (Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()).length > 5000) {
                        Ext.Msg.alert(INFORMATION, ELECONTENTTIP);
                        return;
                    }
                    var isProdValid = true;
                    var msg = 0;
                    if (Ext.getCmp("element_type").getValue().element_type == 3) {
                        //驗證product_id是否合法
                        Ext.Ajax.request({
                            url: '/Element/GetProductInfo',
                            method: 'post',
                            async: false,
                            params: {
                                Product_id: Ext.getCmp("product_id").getValue(),
                                Packet_id: Ext.htmlEncode(Ext.getCmp("packet_id").getValue()),
                                Element_id: Ext.htmlEncode(Ext.getCmp("element_id").getValue())
                            },
                            success: function (form, action) {                            
                                var result = Ext.decode(form.responseText);
                                if (!result.success) {
                                    isProdValid = false;
                                    Ext.Msg.alert(INFORMATION, PRODTIP);
                                }
                                else {
                                    msg = result.msg;
                                    isProdValid = true;
                                }
                            },
                            failure: function () {
                                isProdValid = false;
                                Ext.Msg.alert(INFORMATION, PRODTIP);
                            }
                        });
                    }
                    if (isProdValid) {
                        if (msg == 1) {
                            Ext.MessageBox.confirm(CONFIRM, PRODTIP2, function (btn) {
                                if (btn == "no") {
                                    return false;
                                } else {
                                    form.submit({
                                        params: {
                                            element_id: Ext.htmlEncode(Ext.getCmp("element_id").getValue()),
                                            element_name: Ext.htmlEncode(Ext.getCmp("element_name").getValue()),
                                            element_type: Ext.htmlEncode(Ext.getCmp("element_type").getValue().element_type),
                                            element_content: Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()),
                                            element_product_id: Ext.htmlEncode(Ext.getCmp("product_id").getValue()),
                                            category_id_s: Ext.htmlEncode(Ext.getCmp("category_id").getValue()),
                                            category_name_s: Ext.htmlEncode(Ext.getCmp("category_name").getValue()),
                                            element_link_url: Ext.htmlEncode(Ext.getCmp("element_link_url").getValue()),
                                            element_link_mode: Ext.htmlEncode(Ext.getCmp("element_link_mode").getValue()),
                                            element_sort: Ext.htmlEncode(Ext.getCmp("element_sort").getValue()),
                                            element_start: Ext.htmlEncode(Ext.getCmp("element_start").getValue()),
                                            element_end: Ext.htmlEncode(Ext.getCmp("element_end").getValue()),
                                            element_remark: Ext.htmlEncode(Ext.getCmp("element_remark").getValue()),
                                            packet_id: Ext.htmlEncode(Ext.getCmp("packet_id").getValue()),
                                            element_img_big: Ext.htmlEncode(Ext.getCmp("element_img_big").getValue())
                                            
                                        },
                                        success: function (form, action) {
                                                myMask.hide();
                                            var result = Ext.decode(action.response.responseText);
                                            if (result.success) {
                                                if (result.msg != undefined) {
                                                    Ext.Msg.alert(INFORMATION, result.msg);
                                                }
                                                else {
                                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                                    editWin.close();
                                                    Store.load();
                                                }
                                            } else {
                                                Ext.Msg.alert(INFORMATION, FAILURE);
                                            }
                                        },
                                        failure: function () {
                                            myMask.hide();
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    });
                                }
                            });
                        } else {
                            form.submit({
                                params: {
                                    element_id: Ext.htmlEncode(Ext.getCmp("element_id").getValue()),
                                    element_name: Ext.htmlEncode(Ext.getCmp("element_name").getValue()),
                                    element_type: Ext.htmlEncode(Ext.getCmp("element_type").getValue().element_type),
                                    element_content: Ext.htmlEncode(Ext.getCmp("kendoEditor").getValue()),
                                    element_product_id: Ext.htmlEncode(Ext.getCmp("product_id").getValue()),
                                    category_id_s: Ext.htmlEncode(Ext.getCmp("category_id").getValue()),
                                    category_name_s: Ext.htmlEncode(Ext.getCmp("category_name").getValue()),
                                    element_link_url: Ext.htmlEncode(Ext.getCmp("element_link_url").getValue()),
                                    element_link_mode: Ext.htmlEncode(Ext.getCmp("element_link_mode").getValue()),
                                    element_sort: Ext.htmlEncode(Ext.getCmp("element_sort").getValue()),
                                    element_start: Ext.htmlEncode(Ext.getCmp("element_start").getValue()),
                                    element_end: Ext.htmlEncode(Ext.getCmp("element_end").getValue()),
                                    element_remark: Ext.htmlEncode(Ext.getCmp("element_remark").getValue()),
                                    packet_id: Ext.htmlEncode(Ext.getCmp("packet_id").getValue()),
                                    element_img_big: Ext.htmlEncode(Ext.getCmp("element_img_big").getValue())
                                },
                                success: function (form, action) {
                                    myMask.hide();
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        if (result.msg != undefined) {
                                            Ext.Msg.alert(INFORMATION, result.msg);
                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, SUCCESS);
                                            editWin.close();
                                            Store.load();
                                        }
                                    } else {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                },
                                failure: function () {
                                    myMask.hide();
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });
                        }
                    }
                }

            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '元素詳情',
        id: 'editWin',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        width: 700,
        height: 370,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
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
                PacketStore.load({
                    callback: function () {
                        if (document.getElementById("packetId").value != 0) {
                            var packetid = document.getElementById("packetId").value;
                            Ext.getCmp("packet_id").setValue(packetid);
                            PacketStore.getAt(PacketStore.find("packet_id", packetid));
                            Ext.getCmp("packet_id").setValue(PacketStore.getAt(PacketStore.find("packet_id", packetid)).data.packet_id);
                            //獲取packet中的element_type
                            Ext.Ajax.request({
                                url: '/Element/GetElementTypeInPacket',
                                method: 'post',
                                async: false,
                                params: { PacketId: Ext.getCmp("packet_id").getValue() },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        switch (result.elementType) {
                                            case 1:
                                                Ext.getCmp("isImage").setValue(true);
                                                Ext.getCmp("isContent").setValue(false);
                                                Ext.getCmp("isProduct").setValue(false);
                                                Ext.getCmp("element_type").setReadOnly(true);
                                                break;
                                            case 2:
                                                Ext.getCmp("isContent").setValue(true);
                                                Ext.getCmp("isImage").setValue(false);
                                                Ext.getCmp("isProduct").setValue(false);
                                                Ext.getCmp("element_type").setReadOnly(true);
                                                break;
                                            case 3:
                                                Ext.getCmp("isProduct").setValue(true);
                                                Ext.getCmp("isImage").setValue(false);
                                                Ext.getCmp("isContent").setValue(false);
                                                Ext.getCmp("element_type").setReadOnly(true);

                                                break;
                                            default:
                                                Ext.Msg.alert(INFORMATION, GETPACKETINFOERROR);
                                                break;
                                        }
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, GETPACKETINFOERROR);
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, GETPACKETINFOERROR);
                                }
                            });
                        }
                    }
                });


                if (RowID) {
                    editFrm.getForm().loadRecord(RowID); //如果是編輯的話
                    initForm(RowID);
                }
                else {
                    editFrm.getForm().reset(); //如果不是編輯的話

                }
            }
        }
    });
    editWin.show();
    function initForm(Row) {
        switch (Row.data.element_type) {
            case 1:
                Ext.getCmp("isImage").setValue(true);
                Ext.getCmp("isContent").setValue(false);
                Ext.getCmp("isProduct").setValue(false);
                Ext.getCmp("element_type").setReadOnly(true);
                var img = Row.data.element_content.toString();
                var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
                Ext.getCmp('element_image').setRawValue(imgUrl);
                var img1 = Row.data.element_img_big.toString();
                var imgUrl1 = img1.substring(img1.lastIndexOf("\/") + 1);
                Ext.getCmp('element_img_big').setRawValue(imgUrl1);

                Ext.getCmp("category_id").setValue(Row.data.category_id);
                Ext.getCmp("category_name").setValue(Row.data.category_name)
                break;
            case 2:
                Ext.getCmp("isContent").setValue(true);
                Ext.getCmp("isImage").setValue(false);
                Ext.getCmp("isProduct").setValue(false);
                Ext.getCmp("element_type").setReadOnly(true);
                // alert(Row.data.kendo_editor);
                $('textarea[name=kendoEditor]').data("kendoEditor").value(Row.data.kendo_editor);
                break;
            case 3:
                Ext.getCmp("isProduct").setValue(true);
                Ext.getCmp("isImage").setValue(false);
                Ext.getCmp("isContent").setValue(false);
                Ext.getCmp("element_type").setReadOnly(true);
                var img1 = Row.data.element_img_big.toString();
                var imgUrl1 = img1.substring(img1.lastIndexOf("\/") + 1);
                Ext.getCmp('element_img_big').setRawValue(imgUrl1);
                Ext.getCmp("product_id").setValue(Row.data.product_id);
                Ext.getCmp("category_id").setValue(Row.data.category_id);
                Ext.getCmp("category_name").setValue(Row.data.category_name)
                break;
            default:
                Ext.Msg.alert(INFORMATION, GETPACKETINFOERROR);
                break;
        }
        if (Row.data.category_name != "") {
            Ext.getCmp("category_name").show();
        }

    }

    //時間

    function Tomorrow() {
        var d;
        d = new Date();                             // 创建 Date 对象。
        d.setDate(d.getDate() + 1);
        return d;
    }

}