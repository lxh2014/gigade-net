
editFunction = function (row, store, fatherid, fathername) {
    //前台分類store
    var frontCateStore = Ext.create('Ext.data.TreeStore', {
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/ProductList/GetFrontCatagory',
            actionMethods: 'post'
        },
        root: {
            expanded: true,
            children: []
        }
    });
    frontCateStore.load();


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/ProductCategory/ProductCategorySave',
        items: [
        {
            xtype: 'textfield',
            fieldLabel: CATEGORYID,
            id: 'category_id',
            name: 'category_id',
            submitValue: true,
            hidden: true,
            width: 300
        },
        {
            xtype: 'combotree',
            id: 'comboFrontCage',
            name: 'category_father_name',
            hiddenname: 'category_father_name',
            editable: false,
            submitValue: false,
            colName: 'category_father_name',
            store: frontCateStore,
            fieldLabel: FATHERCATE,
            width: 300,
            labelWidth: 100,
            allowBlank: false
        },
        {
            hidden: true,
            xtype: 'textfield',
            id: 'comboFrontCage_hide',
            name: 'category_father_id',
            width: 10
        },
        {
            xtype: 'textfield',
            fieldLabel: CATEGORYNAME,
            id: 'category_name',
            name: 'category_name',
            submitValue: true,
            hidden: false,
            width: 300,
            allowBlank: false
        },
        {
            xtype: 'numberfield',
            fieldLabel: SORT,
            allowBlank: false,
            id: 'category_sort',
            name: 'category_sort',
            minValue: 0,
            value: 0,
            allowDecimals: false,
            submitValue: true,
            width: 300
        },
        {
            xtype: 'radiogroup',
            hidden: false,
            id: 'category_display',
            name: 'category_display',
            fieldLabel: ISSHOW,
            colName: 'category_display',
            anchor: '100%',
            defaults: {
                name: 'category_display',
                margin: '0 8 0 0'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: SHOWSTATUS, id: 'isShow', inputValue: '1', checked: true },
            { boxLabel: HIDESTATUS, id: 'noShow', inputValue: '0' }
            ]
        }
        ,
        {
            xtype: 'radiogroup',
            hidden: false,
            id: 'category_link_mode',
            name: 'category_link_mode',
            fieldLabel: LINKMODE,
            colName: 'category_link_mode',
            anchor: '100%',
            defaults: {
                name: 'category_link_mode',
                margin: '0 8 0 0'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: OLDWIN, id: 'ls', inputValue: '1', checked: true },
            { boxLabel: NEWWIN, id: 'lm', inputValue: '2' }
            ]
        },
        {
            xtype: 'textfield',
            vtype: 'url',
            fieldLabel: CATELINKURL,
            id: 'category_link_url',
            name: 'category_link_url',
            submitValue: true,
            hidden: false,
            width: 300
        },
        {//Banner
            xtype: 'filefield',
            name: 'photo',
            id: 'photo',
            fieldLabel: CATEBANNER,
            msgTarget: 'side',
            buttonText: SELECT_IMG,
            submitValue: true,
            allowBlank: true,
            fileUpload: true,
            hidden: false,
            width: 300
        },
        {
            xtype: 'radiogroup',
            hidden: false,
            id: 'banner_status',
            name: 'banner_status',
            fieldLabel: BANNERSTATUS,
            colName: 'banner_status',
            defaults: {
                name: 'banner_status',
                margin: '0 8 0 0'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: ACTIVE, id: 'isStatus', inputValue: '1', checked: true },
            { boxLabel: NOTACTIVE, id: 'noStatus', inputValue: '2' }
            ]
        }
        ,
        {
            xtype: 'radiogroup',
            hidden: false,
            id: 'banner_link_mode',
            name: 'banner_link_mode',
            fieldLabel: BANNERLINKMODE,
            colName: 'banner_link_mode',
            defaults: {
                name: 'banner_link_mode',
                margin: '0 8 0 0'
            },
            columns: 2,
            vertical: true,
            items: [
            { boxLabel: OLDWIN, id: 'link_mode1', inputValue: '1', checked: true, width: 150 },
            { boxLabel: NEWWIN, id: 'link_mode12', inputValue: '2' }
            ]
        }
        ,
        {
            xtype: 'textfield',
            fieldLabel: BANNERLINKURL,
            id: 'banner_link_url',
            name: 'banner_link_url',
            submitValue: true,
            hidden: false,
            width: 300,
            vtype: 'url'
        },
        {
            xtype: "datetimefield",
            fieldLabel: BANNERSTART,
            editable: false,
            id: 'startdate',
            name: 'start_date',
            format: 'Y-m-d H:i:s',
            width: 300,
            allowBlank: false,
            submitValue: true,
            time: { hour: 00, min: 00, sec: 00 },
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("startdate");
                    var end = Ext.getCmp("enddate");
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
            id: 'enddate',
            name: 'end_date',
            format: 'Y-m-d H:i:s',
            width: 300,
            allowBlank: false,
            submitValue: true, //
            time: { hour: 23, min: 59, sec: 59 },
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("startdate");
                    var end = Ext.getCmp("enddate");
                    if (end.getValue() < start.getValue()) {
                        Ext.Msg.alert(INFORMATION, TIMETIP);
                        end.setValue("");
                    }
                }
            }
        },
            {
                xtype: 'textareafield',
                fieldLabel: '短文字說明 (300字內)',
                id: 'short_description',
                name: 'short_description',
                width: 300,
                maxLength: 300
            }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            comboFrontCage: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue()),
                            category_id: Ext.htmlEncode(Ext.getCmp('category_id').getValue()),
                            category_name: Ext.htmlEncode(Ext.getCmp('category_name').getValue()),
                            category_sort: Ext.htmlEncode(Ext.getCmp('category_sort').getValue()),
                            category_display: Ext.htmlEncode(Ext.getCmp('category_display').getValue()),
                            categorylinkmode: Ext.htmlEncode(Ext.getCmp('category_link_mode').getValue().category_link_mode),
                            category_link_url: Ext.htmlEncode(Ext.getCmp('category_link_url').getValue()),
                            photo: Ext.htmlEncode(Ext.getCmp('photo').getValue()),
                            banner_status: Ext.htmlEncode(Ext.getCmp('banner_status').getValue().banner_status),
                            banner_link_mode: Ext.htmlEncode(Ext.getCmp('banner_link_mode').getValue().banner_status),
                            banner_link_url: Ext.htmlEncode(Ext.getCmp('banner_link_url').getValue()),
                            startdate: Ext.htmlEncode(Ext.getCmp('startdate').getRawValue()),
                            enddate: Ext.htmlEncode(Ext.getCmp('enddate').getRawValue()),
                            short_description: Ext.htmlEncode(Ext.getCmp('short_description').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SAVESUCCESS);
                                ProductCategoryStore.load({ params: { father_id: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue()) } });
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, SAVEFILURE);
                                ProductCategoryStore.load();
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SAVEFILURE);
                            ProductCategoryStore.load();
                            editWin.close();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: CATEEDIT,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        height: 400,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
        {
            type: 'close',
            qtip: ISCLOSE,
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
        }
        ],
        listeners: {
            'show': function () {
                if (row != null) {
                    editFrm.getForm().loadRecord(row);
                    if (row.data.banner_link_url == "0" || row.data.banner_link_url == null || row.data.banner_link_url == "") {
                        Ext.getCmp("banner_link_url").setValue("");
                    }
                    if (row.data.category_link_url == "0" || row.data.category_link_url == null || row.data.category_link_url == "") {
                        Ext.getCmp("category_link_url").setValue("");
                    }
                    switch (row.data.category_display) {
                        case 1:
                            Ext.getCmp("isShow").setValue(true);
                            break;
                        case 0:
                            Ext.getCmp("noShow").setValue(true);
                            break;
                    }
                    switch (row.data.category_display) {
                        case 1:
                            Ext.getCmp("isShow").setValue(true);
                            break;
                        case 0:
                            Ext.getCmp("noShow").setValue(true);
                            break;
                    };
                    switch (row.data.category_link_mode) {
                        case 1:
                            Ext.getCmp("ls").setValue(true);
                            break;
                        case 0:
                            Ext.getCmp("lm").setValue(true);
                            break;
                    };
                    switch (row.data.banner_status) {
                        case 1:
                            Ext.getCmp("isStatus").setValue(true);
                            break;
                        case 0:
                            Ext.getCmp("noStatus").setValue(true);
                            break;
                    };
                    switch (row.data.banner_link_mode) {
                        case 1:
                            Ext.getCmp("link_mode1").setValue(true);
                            break;
                        case 0:
                            Ext.getCmp("link_mode12").setValue(true);
                            break;
                    };
                    Ext.getCmp('photo').setRawValue(row.data.banner_image);

                    //Ext.getCmp('comboFrontCage_hide').setValue(row.data.category_father_id);
                    Ext.getCmp('comboFrontCage').setValue(row.data.category_father_name);
                    Ext.getCmp('comboFrontCage_hide').setValue(row.data.category_father_id);
                } else {
                    Ext.getCmp('comboFrontCage').setValue(fathername);
                    Ext.getCmp('comboFrontCage_hide').setValue(fatherid);
                }
            }
        }
    });
    editWin.show();
    //時間
    function Tomorrow() {
        var d;
        d = new Date();                             // 创建 Date 对象。
        d.setDate(d.getDate() + 1);
        return d;
    }

}