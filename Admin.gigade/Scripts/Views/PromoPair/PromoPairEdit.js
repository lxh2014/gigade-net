var ID = "";
var even_name = "";
var even_desc = "";
var conditionID = "";
var websiteID = 1;
function editFunction(row, store) {
    VipGroupStore.load();
    if (row != null) {
        ID = row.data["id"];
        conditionID = row.data["condition_id"];
        websiteID = row.data["website"] == "" ? 1 : row.data["website"];

    }
    else {
        SiteStore.load();
    }
    var currentPanel = 0;
    var promoID = "";
    var produCateID = "";
    var produCateID_red = "";
    var produCateID_green = "";
    var navigate = function (panel, direction) {
        // 此程序可以包含一些控制导航步骤的必要业务逻辑. 比如调用setActiveItem, 管理导航按钮的状态,
        // 处理可能出现的分支逻辑, 处理特殊操作像取消或结束等等. 一个完整的向导页, 对于复杂的需求
        // 实现起来可能也会相当复杂, 在实际的程序中通常应该以继承CardLayout的方式来实现. 
        var layout = panel.getLayout();
        var move = Ext.getCmp('move-next').text;
        if ('next' == direction) {
            if (currentPanel == 0) {
                var ffrom = firstForm.getForm();
                if (ffrom.isValid()) {
                    Ext.Ajax.request({
                        url: '/PromoPair/SaveOne',
                        method: 'post',
                        params: { name: Ext.getCmp("event_name").getValue(), desc: Ext.getCmp("event_desc").getValue() },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                promoID = result.id;
                                produCateID = result.cateID;
                                produCateID_red = result.cate_red;
                                produCateID_green = result.cate_green;
                                layout[direction]();
                                currentPanel++;
                                if (!layout.getNext()) {
                                    Ext.getCmp('move-next').hide();
                                    Ext.getCmp('move-prev').setDisabled(false);
                                    Ext.getCmp('move-prev').show();
                                }
                                else {
                                    Ext.getCmp('move-next').setText(NEXT_MOVE);
                                }
                            } else {
                                Ext.Msg.alert(INFORMATION, "操作失敗1");
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "操作失敗2");
                        }
                    });
                }
            } else {
                if (currentPanel == 1) {
                    var condition = Ext.getCmp("condition_id");
                    var group = Ext.getCmp("group_id");
                    var starts = Ext.getCmp("starts");
                    var end = Ext.getCmp("end");

                }
            }
        } else {
            layout[direction]();
            currentPanel--;
        }
        Ext.getCmp('move-next').show();
        Ext.getCmp('move-prev').setDisabled(!layout.getPrev());
    };
    var firstForm = Ext.widget('form', {
        id: 'editFrm1',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                fieldLabel: '活動名稱',
                xtype: 'textfield',
                padding: '10 0 5 0',
                id: 'event_name',
                name: 'event_name',
                allowBlank: false,
                labelWidth: 80
            },
            {
                fieldLabel: '活動描述',
                xtype: 'textfield',
                padding: '10 0 5 0',
                id: 'event_desc',
                name: 'event_desc',
                allowBlank: false,
                labelWidth: 80
            }
        ]
    });
    //第二步
    var secondForm = Ext.widget('form', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        url: '/PromoPair/SaveTwo',
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {

                var form = this.up('form').getForm();
                if (Ext.getCmp("usr2").getValue()) {
                    var text = Ext.getCmp('userInfo1').getValue();
                    if (text == "" || text == null) {
                        alert("請選擇會員條件");
                        return false;
                    }
                }
                if (Ext.getCmp("as1").getValue()) {
                    var text = Ext.getCmp('num_price').getValue();
                    if (text == "" || text == null) {
                        alert("請填寫紅+綠加價");
                        return false;
                    }
                }
                else {
                    var text = Ext.getCmp('num_discount').getValue();
                    if (text == "" || text == null) {
                        alert("請填寫紅+綠打折");
                        return false;
                    }
                }
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            rowid: promoID,
                            categoryid: produCateID,
                            banner: Ext.htmlEncode(Ext.getCmp('banner_image1').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('groupname').getValue()),
                            condition_id: condition_id,
                            deliver_id: Ext.htmlEncode(Ext.getCmp("rd_deliver").getValue().rd_Deliver),
                            device_id: Ext.htmlEncode(Ext.getCmp("rd_device").getValue().rd_Device),
                            price: Ext.htmlEncode(Ext.getCmp('num_price').getValue()),
                            discount: Ext.htmlEncode(Ext.getCmp('num_discount').getValue()),
                            start_date: Ext.htmlEncode(Ext.getCmp('start_date').getValue()),
                            end_date: Ext.htmlEncode(Ext.getCmp('end_date').getValue()),
                            side: Ext.htmlEncode(Ext.getCmp('website').getValue()),
                            vendor_coverage: Ext.htmlEncode(Ext.getCmp('vendor_coverage').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                FaresStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                                form.isValid();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg.toString() == "1") {
                                Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                form.isValid();
                            }
                            else if (result.msg.toString() == "2") {
                                Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                                form.isValid();
                            }
                            else if (result.msg.toString() == "3") {
                                Ext.Msg.alert(INFORMATION, "促銷商品未選擇");
                                form.isValid();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }
                    });
                }
            }
        }],
        defaults: { anchor: "90%", msgTarget: "side" },
        items: [
             {
                 xtype: "numberfield",
                 fieldLabel: VENDORCOVERAGE,
                 id: 'vendor_coverage',
                 name: 'vendor_coverage',
                 allowBlank: false,
                 margin: '5 0 5 0',
                 minValue: 0,
                 value: 0,
                 regex: /^[-+]?([1-9]\d*|0)$/,
                 regexText: VENDERTIP,
                 submitValue: true,
             

             },
            {//專區Banner                
                xtype: 'filefield',
                name: 'banner_image1',
                id: 'banner_image1',
                fieldLabel: BANNERIMG,
                msgTarget: 'side',
                allowBlank: false,
                anchor: '90%',
                buttonText: '選擇...',
                fileUpload: true
            },
            {//會員群組和會員條件二擇一
                xtype: 'fieldcontainer',
                fieldLabel: VIPGROUP,
                combineErrors: true,
                margins: '0 200 0 0',
                layout: 'hbox',
                defaults: {
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'usr',
                        inputValue: "groupname",
                        id: "usr1",
                        checked: true,
                        width: 100,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var com_group = Ext.getCmp("groupname");
                                var btn_group = Ext.getCmp("condi_set");
                                if (newValue) {
                                    btn_group.setDisabled(true);
                                    com_group.setValue("0");
                                    com_group.setDisabled(false);
                                    com_group.allowBlank = false;
                                    Ext.getCmp("userInfo1").hide();
                                    Ext.getCmp("userInfo").hide();
                                }
                            }
                        }
                    },
                    {//會員群組
                        xtype: 'combobox',
                        allowBlank: true,
                        editable: false,
                        hidden: false,
                        id: 'groupname',
                        name: 'group_name',
                        hiddenName: 'group_name',
                        colName: 'groupname',
                        store: VipGroupStore,
                        displayField: 'group_name',
                        valueField: 'group_id',
                        typeAhead: true,
                        forceSelection: false,
                        lastQuery: '',
                        width: 120,
                        value: "0"
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: USERCONDI,
                combineErrors: true,
                layout: 'hbox',
                margins: '0 200 0 0',
                defaults: {
                    width: 120,
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'usr',
                        inputValue: "condi_set",
                        id: "usr2",
                        width: 100,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var com_group = Ext.getCmp("groupname");
                                var btn_group = Ext.getCmp("condi_set");
                                if (newValue) {
                                    com_group.setValue("0");
                                    com_group.allowBlank = true;
                                    com_group.isValid();
                                    com_group.setValue("");
                                    btn_group.setDisabled(false);
                                    com_group.setDisabled(true);
                                    if (conditionID == "") {
                                        Ext.getCmp("userInfo1").hide();
                                    } else if (condition_id != "") {
                                        Ext.getCmp("userInfo1").show();
                                    }
                                }
                            }
                        }
                    },
                    {
                        xtype: 'button',
                        text: CONDINTION,
                        disabled: true,
                        width: 120,
                        id: 'condi_set',
                        colName: 'condi_set',
                        handler: onAddabClick1
                    }
                ]
            },
            {
                xtype: 'textareafield',
                name: 'textarea',
                id: 'userInfo1',
                anchor: '100%',
                hidden: true,
                value: ShowConditionData(conditionID, "userInfo1"),
                listeners: {
                    change: function (textarea, newValue, oldValue) {
                        var textArea = Ext.getCmp("userInfo1");
                        if (newValue != "" && oldValue != "") {
                            textArea.show();
                        }
                        else {
                            textArea.hide();
                        }
                    }
                }
            },
            {
                xtype: 'combobox',
                allowBlank: false,
                fieldLabel: 'web site 設定',
                multiSelect: true, //多選
                hidden: false,
                id: 'website',
                name: 'website',
                hiddenName: 'group_name',
                colName: 'groupname',
                store: SiteStore,
                displayField: 'Site_Name',
                valueField: 'Site_Id',
                queryMode: 'local',
                typeAhead: true,
                forceSelection: false,
                emptyText: 'SELECT',
                listeners: {
                    select: function (a, b, c) {
                        websiteID = Ext.htmlEncode(Ext.getCmp('website').getValue());
                    }
                }
            },
            {//促銷商品類別
                xtype: 'form',
                hidden: false,
                layout: 'hbox',
                border: 0,
                frame: true,
                id: 'p_sortpanl',
                colName: 'user_codi',
                items: [
                    {
                        xtype: 'label',
                        text: PROSORT,
                        id: 'p_sort',
                        margin: '0 8 0 0'
                    },

                    {
                        xtype: 'button',
                        text: '條件設定(紅)',
                        width: 120,
                        id: 'p_condi_red',
                        colName: 'p_condi',
                        name: 'p_condi_red',
                        handler: function () {
                            var regex = /^([0-9]+,)*[0-9]+$/;
                            if (!regex.test(websiteID)) {
                                websiteID = SiteStore.getAt(0).get('Site_id')
                            }
                            PromationMationShow(produCateID_red, websiteID);
                        }
                    },
                    {
                        xtype: 'button',
                        text: '條件設定(綠)',
                        width: 120,
                        id: 'p_condi_green',
                        colName: 'p_condi',
                        name: 'p_condi_green',
                        handler: function () {
                            var regex = /^([0-9]+,)*[0-9]+$/;
                            if (!regex.test(websiteID)) {
                                websiteID = SiteStore.getAt(0).get('Site_id')
                            }
                            PromationMationShow(produCateID_green, websiteID);
                        }
                    }
                ]
            },
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'rd_deliver',
                name: 'rd_deliver',
                fieldLabel: YSCLASS,
                colName: 'rd_deliver',
                width: 400,
                defaults: {
                    name: 'rd_Deliver',
                    margin: '0 8 0 0'
                },
                columns: 3,
                vertical: true,
                items: [
                    { boxLabel: '不分', id: 'bufen1', inputValue: '0', checked: true },
                    { boxLabel: '常溫', id: 'CW', inputValue: '1' },
                    { boxLabel: '冷藏', id: 'LC', inputValue: '2' }
                ]
            },
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'rd_device',
                name: 'rd_device',
                fieldLabel: DEVICE,
                colName: 'rd_device',
                width: 400,
                defaults: {
                    name: 'rd_Device',
                    margin: '0 8 0 0'
                },
                columns: 3,
                vertical: true,
                items: [
                    { boxLabel: '不分', id: 'bufen', inputValue: '0', checked: true },
                    { boxLabel: 'PC', id: 'pc', inputValue: '1' },
                    { boxLabel: '手機/平板', id: 'mobilepad', inputValue: '4' }
                ]
            },
            {  //紅配綠合價
                xtype: 'fieldcontainer',
                fieldLabel: PRICERED,
                combineErrors: true,
                margins: '0 200 0 0',
                layout: 'hbox',
                defaults: {
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'as',
                        inputValue: "u_group",
                        id: "as1",
                        checked: true,
                        width: 100,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var com_group = Ext.getCmp("num_price");
                                var btn_group = Ext.getCmp("num_discount");
                                if (newValue) {
                                    btn_group.setDisabled(true);
                                    com_group.setDisabled(false);
                                    btn_group.setValue("");
                                    com_group.setValue("1");
                                    btn_group.allowBlank = false;
                                    com_group.allowBlank = true;
                                }
                            }
                        }
                    },
                    {//合價             
                        xtype: 'numberfield',
                        id: 'num_price',
                        name: 'num_price',
                        allowBlank: true, //允許為空
                        hidden: false,
                        width: 100,
                        minValue: 1
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: PRICEGREEN,
                combineErrors: true,
                layout: 'hbox',
                margins: '0 200 0 0',
                defaults: {
                    width: 100,
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'as',
                        inputValue: "u_groupset",
                        id: "as2",
                        width: 100,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var rdo_group = Ext.getCmp("as1");
                                var rdo_groppset = Ext.getCmp("as2");
                                var com_group = Ext.getCmp("num_price");
                                var btn_group = Ext.getCmp("num_discount");
                                if (newValue) {
                                    com_group.setValue("");
                                    com_group.allowBlank = true;
                                    btn_group.allowBlank = false;
                                    btn_group.setDisabled(false);
                                    com_group.setValue("");
                                    com_group.setDisabled("true");
                                    btn_group.setValue("1");
                                }
                            }
                        }
                    },
                    {//打折                    
                        xtype: 'numberfield',
                        id: 'num_discount',
                        name: 'num_discount',
                        minValue: 1,
                        allowBlank: true, //允許為空
                        disabled: true,
                        width: 100
                    }
                ]
            },
            {
                xtype: "datetimefield",
                fieldLabel: BEGINTIME,
                id: 'start_date',
                name: 'start_date',
                anchor: '95%',
                format: 'Y-m-d H:i:s',
                time:{hour:00,sec:00,min:00},
                width: 150,
                allowBlank: false,
                submitValue: true,
                value: Tomorrow(),
                enable: false,
                listeners: {
                    select: function (a, b, c) {
                        //var Mounth = new Date(this.getValue()).getMonth() + 1;
                        //Ext.getCmp("end_date").setValue(new Date(new Date(this.getValue()).setMounth(Mounth)));
                        var start = Ext.getCmp("start_date");
                        var end = Ext.getCmp("end_date");
                        var s_date = new Date(start.getValue());

                        //var ttime = s_date;
                        //ttime = new Date(ttime.setMonth(s_date.getMonth() + 1));
                        //ttime = new Date(ttime.setMinutes(59));
                        //ttime = new Date(ttime.setSeconds(59));
                        //ttime = new Date(ttime.setHours(23));

                        end.setValue(setNextMonth(s_date,1));
                    }
                }
            },
            {
                xtype: "datetimefield",
                fieldLabel: ENDTIME,
                id: 'end_date',
                anchor: '95%',
                name: 'end_date',
                format: 'Y-m-d H:i:s',
                time: { hour: 23, sec: 59, min: 59 },
                width: 150,
                allowBlank: false,
                submitValue: true,
                enable: false,
                value: setNextMonth(Tomorrow(),1),
                listeners: {
                    select: function (a, b, c) {
                        //var start = Ext.getCmp("start_date");
                        //var end = Ext.getCmp("end_date");
                        //if (end.getValue() < start.getValue()) {
                        //    Ext.Msg.alert(INFORMATION, TIMETIP);
                        //    end.setValue(new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)));
                        //}
                        var start = Ext.getCmp("start_date");
                        var end = Ext.getCmp("end_date");
                        var s_date = new Date(start.getValue());
                        //var now_date = new Date(end.getValue());
                        if (end.getValue() < start.getValue()) {
                            Ext.Msg.alert(INFORMATION, TIMETIP);
                            end.setValue(setNextMonth(s_date, 1));
                        }
                        //var ttime = now_date;
                        //ttime = new Date(ttime.setMonth(now_date.getMonth()));
                        //ttime = new Date(ttime.setMinutes(59));
                        //ttime = new Date(ttime.setSeconds(59));
                        //ttime = new Date(ttime.setHours(23));

                        //end.setValue(ttime);
                    }
                }
            }
        ]
    });
    var allpan = new Ext.panel.Panel({
        width: 390,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: {
            // 应用到所有子面板
            border: false
        },
        // 这里仅仅用几个按钮来示例一种可能的导航场景.
        bbar: [
        {
            id: 'move-prev',
            text: '上一步',
            handler: function (btn) {
                navigate(btn.up("panel"), "prev");
            },
            disabled: true
        },
        '->', // 一个长间隔, 使两个按钮分布在两边
        {
            id: 'move-next',
            text: '下一步',
            handler: function (btn) {
                navigate(btn.up("panel"), "next");
            }
        }
        ],
        // 布局下的各子面板 //firstForm,secondForm
        items: [firstForm, secondForm]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "紅配綠",
        iconCls: 'icon-user-edit',
        width: 400,
        y: 100,
        constrain: true,
        layout: 'fit',
        items: [allpan],
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
             {
                 type: 'close',
                 qtip: CLOSEFORM,
                 handler: function (event, toolEl, panel) {
                     Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                         if (btn == "yes") {
                             editWin.destroy();
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }],
        listeners: {
            'show': function () {
                firstForm.getForm().reset();
                secondForm.getForm().reset();
            }
        }
    });
    var UpdFrm = Ext.widget('form', {
        id: 'UpdFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        url: '/PromoPair/PromoPairEdit',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
                {
                    fieldLabel: '活動名稱',
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'event_name1',
                    name: 'event_name',
                    allowBlank: false,
                    labelWidth: 80
                },
                {
                    fieldLabel: '活動描述',
                    xtype: 'textfield',
                    padding: '10 0 5 0',
                    id: 'event_desc1',
                    name: 'event_desc',
                    allowBlank: false,
                    labelWidth: 80
                },
                {
                    xtype: "numberfield",
                    fieldLabel: VENDORCOVERAGE,
                    id: 'vendor_coverage1',
                    name: 'vendor_coverage',
                    allowBlank: false,
                    margin: '5 0 5 0',
                    minValue: 0,
                    value: 0,
                    regex: /^[-+]?([1-9]\d*|0)$/,
                    regexText: VENDERTIP,
                    submitValue: true,
                    
                }, 
                {
                    //專區Banner
                    xtype: 'filefield',
                    name: 'banner_image',
                    id: 'banner_image',
                    fieldLabel: '專區Banner',
                    labelWidth: 80,
                    msgTarget: 'side',
                    allowBlank: false,
                    anchor: '90%',
                    buttonText: '選擇...',
                    fileUpload: true
                },
                {
                    //會員群組和會員條件二擇一
                    xtype: 'fieldcontainer',
                    fieldLabel: '會員群組',
                    combineErrors: true,
                    margins: '0 200 0 0',
                    layout: 'hbox',
                    defaults: {
                        hideLabel: true
                    },
                    items: [
                        {
                            xtype: 'radiofield',
                            name: 'us',
                            inputValue: "u_group",
                            id: "us1",
                            checked: true,
                            width: 100,
                            listeners: {
                                change: function (radio, newValue, oldValue) {
                                    var rdo_group = Ext.getCmp("us1");
                                    var rdo_groppset = Ext.getCmp("us2");

                                    var com_group = Ext.getCmp("groupname1");
                                    var btn_group = Ext.getCmp("condi_set1");
                                    if (newValue) {
                                        btn_group.setDisabled(true);
                                        com_group.setValue("0");
                                        com_group.setDisabled(false);
                                        Ext.getCmp("userInfo").hide();
                                        com_group.allowBlank = false;
                                    }
                                }
                            }
                        },
                        {
                            //會員群組
                            xtype: 'combobox',
                            editable: false,
                            hidden: false,
                            id: 'groupname1',
                            name: 'groupname1',
                            hiddenName: 'groupname1',
                            colName: 'groupname1',
                            store: VipGroupStore,
                            displayField: 'group_name',
                            valueField: 'group_id',
                            typeAhead: true,
                            forceSelection: false,
                            allowBlank: false,
                            width: 100,
                            value: "0"
                        }
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    fieldLabel: USERCONDI,
                    combineErrors: true,
                    layout: 'hbox',
                    margins: '0 200 0 0',
                    defaults: {
                        width: 150,
                        hideLabel: true

                    },
                    items: [
                        {
                            xtype: 'radiofield',
                            name: 'us',
                            inputValue: "condi_set1",
                            id: "us2",
                            width: 100,
                            listeners: {
                                change: function (radio, newValue, oldValue) {
                                    var com_group = Ext.getCmp("groupname1");
                                    var btn_group = Ext.getCmp("condi_set1");
                                    if (newValue) {
                                        com_group.setValue("0");
                                        com_group.allowBlank = true;
                                        com_group.setValue("");
                                        com_group.isValid();
                                        btn_group.setDisabled(false);
                                        com_group.setDisabled("true");
                                        if (conditionID == "" || conditionID == "0" || conditionID == 0) {
                                            Ext.getCmp("userInfo").hide();
                                        } else if (condition_id != "") {
                                            Ext.getCmp("userInfo").show();
                                        }
                                    }
                                }
                            }
                        },
                        {
                            xtype: 'button',
                            text: CONDINTION,
                            disabled: true,
                            width: 150,
                            id: 'condi_set1',
                            colName: 'condi_set1',
                            handler: onAddabClick
                        }
                    ]
                },
                {
                    xtype: 'textareafield',
                    name: 'textarea',
                    id: 'userInfo',
                    anchor: '100%',
                    hidden: true,
                    value: ShowConditionData(conditionID, "userInfo"),
                    listeners: {
                        change: function (textarea1, newValue, oldValue) {
                            var textArea = Ext.getCmp("userInfo");
                            if (newValue != "" && oldValue != "") {
                                textArea.show();
                            }
                            else {
                                textArea.hide();
                            }
                        }
                    }
                },
                {
                    xtype: 'combobox',
                    allowBlank: false,
                    editable: false,
                    fieldLabel: 'web site 設定',
                    multiSelect: true, //多選
                    hidden: false,
                    id: 'website1',
                    name: 'website',
                    hiddenName: 'website',
                    colName: 'groupname',
                    store: SiteStore,
                    displayField: 'Site_Name',
                    valueField: 'Site_Id',
                    queryMode: 'local',
                    typeAhead: true,
                    forceSelection: false,
                    emptyText: 'SELECT',
                    listeners: {
                        select: function (a, b, c) {
                            websiteID = Ext.htmlEncode(Ext.getCmp('website').getValue());
                        }
                    }
                },
                {//促銷商品類別                
                    xtype: 'form', //類別設定
                    hidden: false,
                    layout: 'hbox',
                    border: 0,
                    frame: true,
                    id: 'p_sortpanl1',
                    colName: 'user_codi',
                    items: [
                            {
                                xtype: 'label',
                                text: '促銷商品類別',
                                id: 'p_sort',
                                margin: '0 8 0 0'
                            },

                            {
                                xtype: 'button',
                                text: '條件設定(紅)',
                                width: 120,
                                id: 'p_condi_red1',
                                colName: 'p_condi',
                                name: 'p_condi_red',
                                handler: function () {
                                    var regex = /^([0-9]+,)*[0-9]+$/;
                                    if (!regex.test(websiteID)) {
                                        websiteID = SiteStore.getAt(0).get('Site_id')
                                    }
                                    PromationMationShow(produCateID_red, websiteID);
                                }
                            },
                            {
                                xtype: 'button',
                                text: '條件設定(綠)',
                                width: 120,
                                id: 'p_condi_green1',
                                colName: 'p_condi',
                                name: 'p_condi_green',
                                handler: function () {
                                    var regex = /^([0-9]+,)*[0-9]+$/;
                                    if (!regex.test(websiteID)) {
                                        websiteID = SiteStore.getAt(0).get('Site_id')
                                    }
                                    PromationMationShow(produCateID_green, websiteID);
                                }
                            }
                    ]
                },
                {
                    xtype: 'radiogroup',
                    hidden: false,
                    id: 'delivername',
                    name: 'delivername',
                    fieldLabel: '運送類別',
                    colName: 'delivername',
                    width: 400,
                    defaults: {
                        name: 'Deliver',
                        margin: '0 8 0 0'
                    },
                    columns: 3,
                    vertical: true,
                    items: [
                            { boxLabel: '不分', id: 'bufen2', inputValue: '0', checked: true },
                            { boxLabel: '常溫', id: 'CW1', inputValue: '1' },
                            { boxLabel: '冷藏', id: 'LC1', inputValue: '2' }
                    ]
                },
                {
                    xtype: 'radiogroup', //單選框
                    hidden: false, //是否隱藏
                    id: 'device',
                    name: 'device',
                    fieldLabel: '裝置', //控件顯示的字體
                    colName: 'device', //
                    width: 400, //設置的寬度
                    defaults: {
                        name: 'Device',
                        margin: '0 8 0 0'//樣式
                    },
                    columns: 3, //設置顯示的三個單選按鈕
                    vertical: true,
                    items: [
                        { boxLabel: '不分', id: 'bufen3', inputValue: '0', checked: true },
                        { boxLabel: 'PC', id: 'pc1', inputValue: '1' },
                        { boxLabel: '手機/平板', id: 'mobilepad1', inputValue: '4' }]
                },
                {  //紅配綠合價
                    xtype: 'fieldcontainer',
                    fieldLabel: '紅+綠合價',
                    combineErrors: true,
                    margins: '0 200 0 0',
                    layout: 'hbox',
                    defaults: {
                        hideLabel: true
                    },
                    items: [
                        {
                            xtype: 'radiofield',
                            name: 'as',
                            inputValue: "u_group",
                            id: "as3",
                            width: 100,
                            checked: true,
                            listeners: {
                                change: function (radio, newValue, oldValue) {
                                    var rdo_group = Ext.getCmp("as1");
                                    var rdo_groppset = Ext.getCmp("as2");
                                    var com_group = Ext.getCmp("price");
                                    var btn_group = Ext.getCmp("discount");
                                    if (newValue) {
                                        btn_group.setDisabled(true);
                                        com_group.setDisabled(false);
                                        btn_group.setValue("1");
                                        btn_group.allowBlank = true;
                                        btn_group.setValue("");
                                        com_group.setValue("1");
                                        com_group.allowBlank = false;

                                    }
                                }
                            }
                        },
                        {//合價             
                            xtype: 'numberfield',
                            id: 'price',
                            name: 'price',
                            allowBlank: true, //不允許為空
                            hidden: false,
                            width: 100,
                            minValue: 0
                        }
                    ]
                },
                {
                    xtype: 'fieldcontainer',
                    fieldLabel: '紅+綠打折',
                    combineErrors: true,
                    layout: 'hbox',
                    margins: '0 200 0 0',
                    defaults: {
                        width: 100,
                        hideLabel: true
                    },
                    items: [
                        {
                            xtype: 'radiofield',
                            name: 'as',
                            inputValue: "u_groupset",
                            id: "as4",
                            width: 100,
                            listeners: {
                                change: function (radio, newValue, oldValue) {
                                    var rdo_group = Ext.getCmp("as1");
                                    var rdo_groppset = Ext.getCmp("as2");
                                    var com_group = Ext.getCmp("price");
                                    var btn_group = Ext.getCmp("discount");
                                    if (newValue) {
                                        btn_group.setDisabled(false);
                                        btn_group.setValue("1");
                                        com_group.setValue("1");
                                        com_group.allowBlank = true;
                                        com_group.setValue("");
                                        com_group.setDisabled("true");
                                        btn_group.allowBlank = false;

                                    }
                                }
                            }
                        },
                        {
                            xtype: 'numberfield',
                            id: 'discount',
                            name: 'discount',
                            minValue: 0,
                            allowBlank: true,
                            disabled: true,
                            width: 100
                        }
                    ]
                },
                {
                    xtype: "datetimefield",
                    fieldLabel: BEGINTIME,
                    id: 'starts',
                    name: 'starts',
                    anchor: '95%',
                    format: 'Y-m-d H:i:s',
                    time:{hour:00,sec:00,min:00},
                    width: 150,
                    allowBlank: false,
                    submitValue: true,
                    value: Tomorrow(),
                    editable: false,
                    listeners: {
                        select: function (a, b, c) {
                            //var Mounth = new Date(this.getValue()).getMonth() + 1;
                            //Ext.getCmp("end").setValue(new Date(new Date(this.getValue()).setMonth(Mounth)));
                            var start = Ext.getCmp("starts");
                            var end = Ext.getCmp("end");
                            var s_date = new Date(start.getValue());
                            end.setValue(setNextMonth(s_date, 1));
                            //var ttime = s_date;
                            //ttime = new Date(ttime.setMonth(s_date.getMonth() + 1));
                            //ttime = new Date(ttime.setMinutes(59));
                            //ttime = new Date(ttime.setSeconds(59));
                            //ttime = new Date(ttime.setHours(23));

                            //end.setValue(ttime);
                        }
                    }
                },
                {
                    xtype: "datetimefield",
                    fieldLabel: ENDTIME,
                    id: 'end',
                    name: 'end',
                    anchor: '95%',
                    format: 'Y-m-d H:i:s',
                    time: { hour: 23, sec: 59, min: 59 },
                    width: 150,
                    allowBlank: false,
                    editable: false,
                    submitValue: true,
                    listeners: {
                        select: function (a, b, c) {
                            //var start = Ext.getCmp("starts");
                            //var end = Ext.getCmp("end");
                            //if (end.getValue() < start.getValue()) {
                            //    Ext.Msg.alert(INFORMATION, TIMETIP);
                            //    Ext.getCmp("end").setValue(new Date(new Date(start.getValue()).setMonth(new Date(start.getValue()).getMonth() + 1)));
                            //}
                            var start = Ext.getCmp("starts");
                            var end = Ext.getCmp("end");
                            var s_date = new Date(start.getValue());
                            //var now_date = new Date(end.getValue());
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, TIMETIP);
                                end.setValue(setNextMonth(s_date, 1));
                            }
                            //var ttime = now_date;
                            //ttime = new Date(ttime.setMonth(now_date.getMonth()));
                            //ttime = new Date(ttime.setMinutes(59));
                            //ttime = new Date(ttime.setSeconds(59));
                            //ttime = new Date(ttime.setHours(23));

                            //end.setValue(ttime);
                        }
                    }

                }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {

                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            rowid: promoID,
                            categoryid: produCateID,
                            event_name: Ext.getCmp('event_name1').getValue(),
                            event_desc: Ext.getCmp('event_desc1').getValue(),
                            banner: Ext.htmlEncode(Ext.getCmp('banner_image').getValue()),
                            group_id: Ext.htmlEncode(Ext.getCmp('groupname1').getValue()),
                            condition_id: condition_id,
                            deliver_id: Ext.htmlEncode(Ext.getCmp("delivername").getValue().Deliver),
                            device_id: Ext.htmlEncode(Ext.getCmp("device").getValue().Device),
                            price: Ext.htmlEncode(Ext.getCmp('price').getValue()),
                            discount: Ext.htmlEncode(Ext.getCmp('discount').getValue()),
                            starts: Ext.htmlEncode(Ext.getCmp('starts').getValue()),
                            end: Ext.htmlEncode(Ext.getCmp('end').getValue()),
                            side: Ext.htmlEncode(Ext.getCmp('website1').getValue()),
                            vendor_coverage:Ext.htmlEncode(Ext.getCmp('vendor_coverage1').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                FaresStore.load();
                                Updwin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                                form.isValid();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);

                            if (result.msg.toString() == "1") {
                                Ext.Msg.alert(INFORMATION, PHOTOSIZE);
                                form.isValid();
                            }
                            else if (result.msg.toString() == "2") {
                                Ext.Msg.alert(INFORMATION, PHOTOTYPE);
                                form.isValid();
                            }
                            else if (result.msg.toString() == "3") {
                                Ext.Msg.alert(INFORMATION, "促銷商品未選擇!");
                                form.isValid();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }
                    });
                }
            }
        }]
    });
    var Updwin = Ext.create('Ext.window.Window', {
        title: "紅配綠",
        iconCls: 'icon-user-edit',
        width: 400,
        y: 100,
        constrain: true,
        layout: 'fit',
        items: [UpdFrm],
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
             {
                 type: 'close',
                 qtip: CLOSEFORM,
                 handler: function (event, toolEl, panel) {
                     Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                         if (btn == "yes") {
                             Updwin.destroy();
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }],
        listeners: {
            'show': function () {
                UpdFrm.getForm().loadRecord(row);
                promoID = row.data.id;
                var devi = row.data.device;
                produCateID = row.data.category_id;
                produCateID_red = row.data.cate_red;
                produCateID_green = row.data.cate_green;
                Ext.getCmp('banner_image').setRawValue(row.data.banner_image);

                if (row.data.condition_id != 0) {
                    Ext.getCmp('us1').setValue(false);
                    Ext.getCmp('us2').setValue(true);
                }
                else {
                    Ext.getCmp('us1').setValue(true);
                    Ext.getCmp('us2').setValue(false);
                    if (row.data.group_name == "") {
                        Ext.getCmp('groupname1').setValue(BUFEN);
                    }
                    else {
                        Ext.getCmp('groupname1').setValue(row.data.group_name);
                    }
                }
                var deli = row.data.deliver_type;
                if (devi == 0) {
                    Ext.getCmp("bufen3").setValue(true);
                    Ext.getCmp("pc1").setValue(false);
                    Ext.getCmp("mobilepad1").setValue(false);
                } else if (devi == 1) {
                    Ext.getCmp("bufen3").setValue(false);
                    Ext.getCmp("pc1").setValue(true);
                    Ext.getCmp("mobilepad1").setValue(false);
                } else if (devi == 4) {
                    Ext.getCmp("bufen3").setValue(false);
                    Ext.getCmp("pc1").setValue(false);
                    Ext.getCmp("mobilepad1").setValue(true);
                }
                if (deli == 0) {
                    Ext.getCmp("bufen2").setValue(true);
                    Ext.getCmp("CW1").setValue(false);
                    Ext.getCmp("LC1").setValue(false);
                }
                else if (deli == 1) {
                    Ext.getCmp("bufen2").setValue(false);
                    Ext.getCmp("CW1").setValue(true);
                    Ext.getCmp("LC1").setValue(false);
                }
                else if (deli == 2) {
                    Ext.getCmp("bufen2").setValue(false);
                    Ext.getCmp("CW1").setValue(false);
                    Ext.getCmp("LC1").setValue(true);
                }

                var pri = row.data.price; //紅+綠 合價and 打折 二選一
                var dis = row.data.discount;
                if (pri != 0) {
                    Ext.getCmp("as3").setValue(true);
                    Ext.getCmp("as4").setValue(false);
                    Ext.getCmp("price").setValue(pri);
                }
                else if (dis != 0) {
                    Ext.getCmp("as3").setValue(false);
                    Ext.getCmp("as4").setValue(true);
                    Ext.getCmp("discount").setValue(dis);
                }
                if (row.data.banner_image != "") {
                    var img = row.data.banner_image.toString();
                    var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
                    Ext.getCmp('banner_image').setRawValue(imgUrl);
                }
                //修改時對site賦值
                //var siteIDs = row.data.website.toString().split(',');
                //var combobox = Ext.getCmp('website1');
                //var store = combobox.store;
                //var arrTemp = new Array();
                //var length = store.count();
                //for (var i = 0; i < siteIDs.length; i++) {
                //    arrTemp.push(store.getAt(store.find("Site_Id", siteIDs[i])));
                //}
                //combobox.setValue(arrTemp);
                //修改時對site賦值
                SiteStore.load({
                    callback: function () {
                        var siteIDs = row.data.website.toString().split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < siteIDs.length; i++) {
                            arrTemp.push(SiteStore.getAt(SiteStore.find("Site_Id", siteIDs[i])));
                        }
                        Ext.getCmp('website1').setValue(arrTemp);

                    }
                })
            }
        }
    });
    if (row != null) {
        Updwin.show();
    } else {
        editWin.show();
    }
}
onAddabClick = function () {
    showUserSetForm(null, conditionID, "userInfo");
}
onAddabClick1 = function () {
    showUserSetForm(null, conditionID, "userInfo1");
}

function setNextMonth(source, n) {
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