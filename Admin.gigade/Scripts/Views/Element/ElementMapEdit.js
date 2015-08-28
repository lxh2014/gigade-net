editFunction = function (row, store) {
    //area_packet
    Ext.define("gigade.mapPacket", {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'packet_id', type: 'string' },
            { name: 'packet_name', type: 'string' }
        ]
    });

    var mapPacketStore = Ext.create('Ext.data.Store', {
        model: 'gigade.mapPacket',
        autoLoad: false,
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

    mapPacketStore.on('beforeload', function () {
        Ext.apply(mapPacketStore.proxy.extraParams, {
            ele_type: Ext.htmlEncode(AreaStore.getAt(AreaStore.find("area_id", Ext.getCmp("area_id").getValue())).data.element_type)

        });
    });



    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Element/SaveElementMap',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: BANNERID,
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
            fieldLabel: SITENAME,
            store: SiteStore,
            emptyText: SELECT,
            //queryMode: 'local',
            lastQuery: '',
            displayField: 'Site_Name',
            valueField: 'Site_Id',
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
            lastQuery: '',
            allowBlank: false,
            fieldLabel: PAGENAME,
            store: PageStore,
            emptyText: SELECT,
            //  queryMode: 'local',
            displayField: 'page_name',
            valueField: 'page_id',
            typeAhead: true,
            forceSelection: false,
            editable: false
        },
        {
            xtype: 'combobox',
            allowBlank: true,
            hidden: false,
            id: 'area_id',
            name: 'area_id',
            lastQuery: '',
            allowBlank: false,
            fieldLabel: AREANAME,
            store: AreaStore,
            emptyText: SELECT,
            displayField: 'area_name',
            valueField: 'area_id',
            typeAhead: true,
            forceSelection: false,
            editable: false,
            listeners: {
                select: function (combo, records, eOpts) {
                    Ext.Ajax.request({//根據area_id查詢出該區域的element_type
                        url: '/Element/GetAreaCount',
                        method: 'post',
                        async: false,
                        params: {
                            AreaId: Ext.getCmp("area_id").getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (!result.success) {
                                Ext.Msg.alert(INFORMATION, PACKETNUMBER);
                               // Ext.getCmp('editWin').destroy();
                                Ext.getCmp('area_id').setValue("");
                            }
                        }
                    });
                },
                change: function (combo, records, eOpts) {
                    Ext.getCmp("packet_id").setValue("");

                    //mapPacketStore.removeAll();
                    var area_id = Ext.getCmp("area_id").getValue();
                    var eleType = AreaStore.getAt(AreaStore.find("area_id", area_id)).data.element_type;
                    mapPacketStore.load({ params: { ele_type: eleType } });

                }
            }

        },
{
    xtype: 'combobox',
    hidden: false,
    id: 'packet_id',
    name: 'packet_id',
    lastQuery: '',
    allowBlank: false,
    fieldLabel: PACKETNAME,
    store: mapPacketStore,
    emptyText: SELECT,
    displayField: 'packet_name',
    valueField: 'packet_id',
    editable: false,
    listeners: {
        beforequery: function (qe) {
            mapPacketStore.removeAll();
            var area_id = Ext.getCmp("area_id").getValue();
            var eleType = AreaStore.getAt(AreaStore.find("area_id", area_id)).data.element_type;

            mapPacketStore.load({ params: { ele_type: eleType } });


        }
    }
},
{
    xtype: 'numberfield',
    fieldLabel: SORT,
    name: 'sort',
    id: 'sort',
    allowBlank: false,
    value: 0,
    minValue: 0
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
                            map_id: Ext.htmlEncode(Ext.getCmp('map_id').getValue()),
                            site_id: Ext.htmlEncode(Ext.getCmp('site_id').getValue()),
                            page_id: Ext.htmlEncode(Ext.getCmp('page_id').getValue()),
                            area_id: Ext.htmlEncode(Ext.getCmp('area_id').getValue()),
                            packet_id: Ext.htmlEncode(Ext.getCmp('packet_id').getValue()),
                            sort: Ext.htmlEncode(Ext.getCmp('sort').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                ElementMapStore.load();
                                //  ElementMapStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, MAPFAILURE);
                                ElementMapStore.load();
                                // ElementMapStore.load();
                                editWin.close();
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, MAPFAILURE);
                            ElementMapStore.load();
                            // ElementMapStore.load();
                            editWin.close();
                            // ElementMapStore.load();
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: MAPTITLE,
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
         }
        ],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();
}
