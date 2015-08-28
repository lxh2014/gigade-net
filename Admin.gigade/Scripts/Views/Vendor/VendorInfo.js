VendorInfoFunction = function (row) {
    var VendorID = null;
    if (row !== null) {
        VendorID = row.data.vendor_id;
    }
    //聯繫人
    Ext.define("gigade.connecttype", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "value", type: "string" }
        ]
    });
    var connecttypeStore = Ext.create('Ext.data.Store', {
        model: 'gigade.connecttype',
        autoLoad: true,
        data: [
            { value: '請選擇' },
            { value: '負責人' },
            { value: '業務窗口' },
            { value: '圖/文窗口' },
            { value: '出貨負責窗口' },
            { value: '帳務連絡窗口' },
            { value: '客服窗口' }
        ]
    });

    Ext.define('Contact', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "rid", type: "string" },
            { name: "contact_type", type: "string" },
            { name: "contact_name", type: "string" },
            { name: "contact_phone1", type: "string" },
            { name: "contact_phone2", type: "string" },
            { name: "contact_mobile", type: "string" },
            { name: "contact_email", type: "string" }
        ]
    });

    var ContactStoretwo = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'Contact',
        remoteSort: false,
        autoLoad: false,
        proxy: {
            type: 'ajax',
            //url: "/Vendor/QueryContact",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });

    var editFrm1 = Ext.create('Ext.grid.Panel', {
        id: 'gdcontact',
        store: ContactStoretwo,
        anchor: '98%',
        height: 220,
        autoScroll: true,
        frame: true,
        columns: [
            {
                header: '窗口屬性',
                dataIndex: 'contact_type',
                width: 90,
                align: 'left',
                editor: {
                    id: 'txt_contacttype',
                    xtype: 'combobox',
                    typeAhead: true,
                    store: connecttypeStore,
                    displayField: 'value',
                    valueField: 'value',
                    typeAhead: true,
                    value: 0,
                    forceSelection: false,
                    queryMode: 'local'
                }
            },
            { header: '姓名', dataIndex: 'contact_name', width: 120, align: 'center' },
            {
                header: '電話1', dataIndex: 'contact_phone1', width: 100, align: 'center'
            },
            {
                header: '電話2', dataIndex: 'contact_phone2', width: 100, align: 'center'
            },
            {
                header: '手機', dataIndex: 'contact_mobile', width: 100, align: 'center'
            },
            {
                header: 'E-mail', dataIndex: 'contact_email', width: 150, align: 'center'
            }


        ]

    });


    var editWintwo = Ext.create('Ext.window.Window', {
        title: '供應商聯絡人',
        iconCls: 'icon-user-edit',
        id: 'editWintwo',
        width: 700,
        layout: 'fit',
        height: 300,
        items: [editFrm1],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWintwo').destroy();
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
                if (row) {
                    initForm(row);
                }
                else {
                }
            }
        }
    });
    editWintwo.show();
    function initForm(row) {
        if (row.data.contact_type_1 !== 0) {
            ContactStoretwo.removeAll();
            ContactStoretwo.add({
                contact_type: '出貨聯絡窗口',
                contact_name: row.data.contact_name_1,
                contact_phone1: row.data.contact_phone_1_1,
                contact_phone2: row.data.contact_phone_2_1,
                contact_mobile: row.data.contact_mobile_1,
                contact_email: row.data.contact_email_1
            });
        }
        if (row.data.contact_type_2 !== 0) {
            ContactStoretwo.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_2].get("value"),
                contact_name: row.data.contact_name_2,
                contact_phone1: row.data.contact_phone_1_2,
                contact_phone2: row.data.contact_phone_2_2,
                contact_mobile: row.data.contact_mobile_2,
                contact_email: row.data.contact_email_2
            });
        }
        if (row.data.contact_type_3 !== 0) {
            ContactStoretwo.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_3].get("value"),
                contact_name: row.data.contact_name_3,
                contact_phone1: row.data.contact_phone_1_3,
                contact_phone2: row.data.contact_phone_2_3,
                contact_mobile: row.data.contact_mobile_3,
                contact_email: row.data.contact_email_3
            });
        }
        if (row.data.contact_type_4 !== 0) {
            ContactStoretwo.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_4].get("value"),
                contact_name: row.data.contact_name_4,
                contact_phone1: row.data.contact_phone_1_4,
                contact_phone2: row.data.contact_phone_2_4,
                contact_mobile: row.data.contact_mobile_4,
                contact_email: row.data.contact_email_4
            });
        }
        if (row.data.contact_type_5 !== 0) {
            ContactStoretwo.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_5].get("value"),
                contact_name: row.data.contact_name_5,
                contact_phone1: row.data.contact_phone_1_5,
                contact_phone2: row.data.contact_phone_2_5,
                contact_mobile: row.data.contact_mobile_5,
                contact_email: row.data.contact_email_5
            });
        }
    }

};