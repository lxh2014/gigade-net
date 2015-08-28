var channel_id

//聯絡人Model
Ext.define('Contact', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rid", type: "string" },
        { name: "contact_type", type: "string" },
        { name: "contact_name", type: "string" },
        { name: "contact_phone1", type: "string" },
        { name: "contact_phone2", type: "string" },
        { name: "contact_mobile", type: "string" },
        { name: "contact_email", type: "string"}]
});

var ContactStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'Contact',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryContact",
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
ContactStore.on('beforeload', function () {
    Ext.apply(ContactStore.proxy.extraParams,
            {
                channel_id: 0
            });
});
var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});


Ext.onReady(function () {
    var gdContact = Ext.create('Ext.grid.Panel', {
        id: 'gdContact',
        store: ContactStore,
        plugins: [cellEditing],
        width: document.documentElement.clientWidth,
        frame: true,
        columns: [
            { header: GONGNENG, xtype: 'actioncolumn', width: 150, align: 'center',
                items: [{
                    icon: '../../../Content/img/icons/cross.gif',
                    tooltip: DELETE,
                    handler: function (grid, rowIndex, colIndex) {
                        if (!confirm(YDELETE)) {
                            return;
                        }
                        var rec = gdContact.getStore().getAt(rowIndex);
                        Ext.Ajax.request({
                            url: '/Channel/DeleteContact',
                            params: {
                                rid: rec.get('rid')
                            },
                            success: function (response) {
                                var result = Ext.decode(response.responseText);
                                Ext.Msg.alert(MESSAGE, result.msg);
                                ContactStore.removeAll();
                                ContactStore.load({
                                    params: {
                                        channel_id: window.parent.getChannelId()
                                    }
                                });
                            },
                            failure: function (response) {
                                var result = Ext.decode(response.responseText);
                                Ext.Msg.alert(MESSAGE, result.msg);
                            }
                        });
                    }
                }]
            },
            { header: 'ID', dataIndex: 'rid', align: 'center', hidden: true },
            { header: CONTACTTYPE, dataIndex: 'contact_type', width: 150, align: 'center', editor: { id: 'txt_contacttype'} },
            { header: CONTACTNAME, dataIndex: 'contact_name', width: 150, align: 'center', editor: { id: 'txt_contactname'} },
            { header: CONTACTPHONE1, dataIndex: 'contact_phone1', width: 150, align: 'center', editor: { id: 'txt_contactphone'} },
            { header: CONTACTPHONE2, dataIndex: 'contact_phone2', width: 150, align: 'center', editor: { id: 'txt_contactphone1'} },
            { header: CONTACTMOBILE, dataIndex: 'contact_mobile', width: 150, align: 'center', editor: { id: 'txt_contactmobile'} },
            { header: CONTACTEMAIL, dataIndex: 'contact_email', width: 350, align: 'center', editor: { id: 'txt_contactemail'} }
        ],
        tbar: [{ xtype: 'button', text: INSERTCONTACT, iconCls: 'icon-user-add', handler: onAddClick}],
        buttonAlign: 'left',
        buttons: [
            {
                text: SAVE,
                id: 'btn_submit',
                handler: function () {
                    if (window.parent.getChannelId() == "") {
                        alert(NEXT_INSERT);
                        return;
                    }

                    var gdContact = Ext.getCmp("gdContact").store.data.items;
                    var InsertValues = "";
                    for (var a = 0; a < gdContact.length; a++) {
                        var rid = gdContact[a].get("rid");
                        var type = gdContact[a].get("contact_type");
                        var name = gdContact[a].get("contact_name");
                        var phone1 = gdContact[a].get("contact_phone1");
                        var phone2 = gdContact[a].get("contact_phone2");
                        var mobile = gdContact[a].get("contact_mobile");
                        var email = gdContact[a].get("contact_email");

                        var reg = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$|^\s*$/;

                        if (!reg.test(email)) {
                            alert(ERROR_EMAIL);
                            return;
                        }

                        InsertValues += rid + ',' + type + ',' + name + ',' + phone1 + ',' + phone2 + ',' + mobile + ',' + email + ';'
                    }

                    Ext.Ajax.request({
                        url: '/Channel/SaveChannelContact',
                        params: {
                            'Channel_id': window.parent.getChannelId(),
                            'InsertValues': InsertValues
                        },
                        success: function (response, opts) {
                            var result = eval("(" + response.responseText + ")");
                            Ext.Msg.alert(MESSAGE, result.msg);
                            ContactStore.removeAll();
                            ContactStore.load({
                                params: {
                                    channel_id: window.parent.getChannelId()
                                }
                            });
                        },
                        failure: function (response) {
                            var result = eval("(" + response.responseText + ")");
                            Ext.Msg.alert(MESSAGE, result.msg);
                        }
                    });
                }
            }, {
                text: RETURN,
                hidden: window.parent.getChannelId() == '',
                handler: function () {
                    window.parent.location.href = 'http://' + window.parent.location.host + '/channel/channellist';
                }
            }]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdContact],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdContact.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

    channel_id = window.parent.getChannelId();
    if (channel_id != "") {

        ContactStore.load({
            params: {
                channel_id: channel_id
            }
        });
    }
});

function setDisable(flag) {
    Ext.getCmp("btn_submit").setDisabled(flag);
}

function onAddClick() {
    var r = Ext.create('Contact', {
        contact_type: '',
        contact_name: '',
        contact_phone1: '',
        contact_phone2: '',
        contact_mobile: '',
        contact_email: ''
    });
    ContactStore.insert(0, r);
    cellEditing.startEditByPosition({ row: 0, column: 2 });
}