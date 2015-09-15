editFunction = function (row, store) {
    Ext.define("gigade.Site", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "Site_Id", type: "string" },
            { name: "Site_Name", type: "string" }]
    });

    var SiteStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Site',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Element/GetSite",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });
    //page
    Ext.define("gigade.Page", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "page_id", type: "string" },
            { name: "page_name", type: "string" }]
    });

    var PageStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Page',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Element/GetPage",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });
    //area
    Ext.define("gigade.Area", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "area_id", type: "string" },
            { name: "area_name", type: "string" },
             { name: "element_type", type: "string" }
        ]
    });

    var AreaStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Area',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Element/GetArea",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });

    //area
    Ext.define("gigade.Info", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "info_id", type: "string" },
            { name: "info_name", type: "string" }

        ]
    });

    var info_epaper = Ext.create('Ext.data.Store', {
        model: 'gigade.Info',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/InfoMap/GetEpaperContent",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });

    var info_news = Ext.create('Ext.data.Store', {
        model: 'gigade.Info',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/InfoMap/GetNewsContent",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    var info_announce = Ext.create('Ext.data.Store', {
        model: 'gigade.Info',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/InfoMap/GetAnnounce",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });

    var info_edm = Ext.create('Ext.data.Store', {
        model: 'gigade.Info',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/InfoMap/GetEdmContent",
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
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        height: 210,
        layout: 'anchor',
        labelWidth: 45,
        url: '/InfoMap/SaveInfoMap',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: 'Mapid',
            id: 'map_id',
            name: 'map_id',
            submitValue: true,
            hidden: true
        },
        {
            xtype: 'combobox',
            allowBlank: true,
            hidden: false,
            id: 'site_id',
            name: 'site_id',
            allowBlank: false,
            fieldLabel: SITE,
            store: SiteStore,
            emptyText: SELECT,
            displayField: 'Site_Name',
            valueField: 'Site_Id',
            lastQuery: '',
            typeAhead: true,
            forceSelection: false,
            editable: false
        },
        {
            xtype: 'combobox',
            allowBlank: true,
            hidden: false,
            id: 'page_id',
            name: 'page_id',
            allowBlank: false,
            fieldLabel: PAGE,
            store: PageStore,
            emptyText: SELECT,
            displayField: 'page_name',
            valueField: 'page_id',
            typeAhead: true,
            forceSelection: false,
            lastQuery: '',
            editable: false
        },
        {
            xtype: 'combobox',
            allowBlank: true,
            hidden: false,
            id: 'area_id',
            name: 'area_id',
            allowBlank: false,
            fieldLabel: AREA,
            store: AreaStore,
            emptyText: SELECT,
            displayField: 'area_name',
            valueField: 'area_id',
            typeAhead: true,
            lastQuery: '',
            forceSelection: false,
            editable: false
        },
        {
            xtype: 'combobox', //元素類型
            allowBlank: false,
            fieldLabel: TYPE,
            editable: false,
            id: 'type',
            name: 'type',
            store: InfoTypeStore,
            displayField: 'type_name',
            valueField: 'type_id',
            typeAhead: true,
            forceSelection: false,
            emptyText: SELECT,
            lastQuery: '',
            queryMode: 'local',
            listeners: {
                change: function (combo, newValue, oldValue) {
                    if (newValue) {
                        var type = Ext.getCmp('type').getValue();
                        var info1 = Ext.getCmp('info_id1');
                        var info2 = Ext.getCmp('info_id2');
                        var info3 = Ext.getCmp('info_id3');
                        var info4 = Ext.getCmp('info_id4');
                        info1.allowBlank = true;
                        info2.allowBlank = true;
                        info3.allowBlank = true;
                        info4.allowBlank = true;
                        info1.setValue("").hide();
                        info2.setValue("").hide();
                        info3.setValue("").hide();
                        info4.setValue("").hide();

                        switch (type) {
                            case '1':
                                info1.setValue("0");
                                info1.allowBlank = false;
                                info1.setValue("");
                                info1.show();

                                break;
                            case '2':
                                info2.setValue("0");
                                info2.allowBlank = false;
                                info2.setValue("");
                                info2.show();

                                break;
                            case '3':
                                info3.setValue("0");
                                info3.allowBlank = false;
                                info3.setValue("");
                                info3.show();

                                break;
                            case '4':
                                info4.setValue("0");
                                info4.allowBlank = false;
                                info4.setValue("");
                                info4.show();

                                break;
                        }
                    }
                }

            }
        },
        {
            xtype: 'combobox',
            id: 'info_id1',
            name: 'info_id',
            //  allowBlank: true,
            fieldLabel: INFONAME,
            store: info_epaper,
            emptyText: SELECT,
            displayField: 'info_name',
            valueField: 'info_id',
            typeAhead: true,
            forceSelection: false,
            hidden: true,
            lastQuery: '',
            editable: false
        },
        {
            xtype: 'combobox',
            id: 'info_id2',
            name: 'info_id',
            //   allowBlank: true,
            fieldLabel: INFONAME,
            store: info_news,
            emptyText: SELECT,
            displayField: 'info_name',
            valueField: 'info_id',
            typeAhead: true,
            forceSelection: false,
            hidden: true,
            lastQuery: '',
            editable: false
        },
        {
            xtype: 'combobox',
            id: 'info_id3',
            name: 'info_id',
            //   allowBlank: true,
            fieldLabel: INFONAME,
            store: info_announce,
            emptyText: SELECT,
            displayField: 'info_name',
            valueField: 'info_id',
            typeAhead: true,
            forceSelection: false,
            hidden: true,
            lastQuery: '',
            editable: false
        },
        {
            xtype: 'combobox',
            id: 'info_id4',
            name: 'info_id',
            //  allowBlank: true,
            fieldLabel: INFONAME,
            store: info_edm,
            emptyText: SELECT,
            displayField: 'info_name',
            valueField: 'info_id',
            typeAhead: true,
            forceSelection: false,
            hidden: true,
            lastQuery: '',
            editable: false
        },
      {
          xtype: 'numberfield',
          fieldLabel: SORT,
          name: 'sort',
          id: 'sort',
          // allowBlank: false,
          value: 0,
          minValue: 0
      }

        ],
        buttons: [{
            formBind: true,
            text: SAVE,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    form.submit({
                        params: {
                            map_id: Ext.htmlEncode(Ext.getCmp('map_id').getValue()),
                            site_id: Ext.htmlEncode(Ext.getCmp('site_id').getValue()),
                            page_id: Ext.htmlEncode(Ext.getCmp('page_id').getValue()),
                            area_id: Ext.htmlEncode(Ext.getCmp('area_id').getValue()),
                            type: Ext.htmlEncode(Ext.getCmp('type').getValue()),
                            info_id1: Ext.htmlEncode(Ext.getCmp('info_id1').getValue()),
                            info_id2: Ext.htmlEncode(Ext.getCmp('info_id2').getValue()),
                            info_id3: Ext.htmlEncode(Ext.getCmp('info_id3').getValue()),
                            info_id4: Ext.htmlEncode(Ext.getCmp('info_id4').getValue()),
                            sort: Ext.htmlEncode(Ext.getCmp('sort').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                InfoMapStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                                InfoMapStore.load();
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, FAILURE);
                            InfoMapStore.load();
                            editWin.close();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: WINTITLE,
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
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
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話
                } else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                    inintFrm(row);
                }
            }
        }
    });
    editWin.show();

    function inintFrm(Row) {

        var type = Row.data.type;
        var info1 = Ext.getCmp('info_id1');
        var info2 = Ext.getCmp('info_id2');
        var info3 = Ext.getCmp('info_id3');
        var info4 = Ext.getCmp('info_id4');


        info1.allowBlank = true;
        info2.allowBlank = true;
        info3.allowBlank = true;
        info4.allowBlank = true;

        switch (type) {
            case 1:
                info1.setValue(Row.data.info_id);
                info1.allowBlank = false;
                info1.show();

                break;
            case 2:
                info2.setValue(Row.data.info_id);
                info2.allowBlank = false;
                info2.show();

                break;
            case 3:
                info3.setValue(Row.data.info_id);
                info3.allowBlank = false;
                info3.show();

                break;
            case 4:
                info4.setValue(Row.data.info_id);
                info4.allowBlank = false;
                info4.show();

                break;
        }

    }
}
