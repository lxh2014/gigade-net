
//聯絡人gridModel
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

var ContactStore = Ext.create('Ext.data.Store', {
    //autoDestroy: true,
    model: 'Contact',
    //remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Vendor/QueryContactTable",
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

var gdcontact = Ext.create('Ext.grid.Panel', {
    id: 'gdcontact',
    store: ContactStore,
 
    anchor: '98%',
    height: 160,
    autoScroll: true,
   // frame: true,
    columns: [
        { header: '窗口屬性', dataIndex: 'contact_type', width: 110, align: 'center' },
        { header: '姓名', dataIndex: 'contact_name', width: 100, align: 'center' },
        { header: '電話1', dataIndex: 'contact_phone1', width: 100, align: 'center'},
        { header: '電話2', dataIndex: 'contact_phone2', width: 100, align: 'center' },
        { header: '手機', dataIndex: 'contact_mobile', width: 100, align: 'center' },
        { header: 'E-mail', dataIndex: 'contact_email', width: 90, align: 'center'}
    ]
    
});



