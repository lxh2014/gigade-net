Ext.define('GIGADE.EmsGoalCom', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'department_code', type: 'string' },
        { name: 'department_name', type: 'string' }
    ]
});
var EmsGoalComStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    model: 'GIGADE.EmsGoalCom',
    proxy: {
        type: 'ajax',
        url: '/Ems/GetDepartmentStore',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});


Ext.define('GIGADE.EmsDepRe', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'dep_code', type: 'string' },
        { name: 'dep_name', type: 'string' }
    ]
});
var EmsDepStore = Ext.create("Ext.data.Store", {
    autoDestroy: true,
    model: 'GIGADE.EmsDepRe',
    proxy: {
        type: 'ajax',
        url: '/EmsDepRelation/GetDepStore',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});


var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "所有日期", "value": "0" },
        { "txt": "特定日期", "value": "1" }
    ]
});
 
var relationTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
         { "txt": "請選擇...", "value": "0" },
        { "txt": "公關單", "value": "1" },
        { "txt": "報廢單", "value": "2" }
    ]
});

var reTypeStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
         { "txt": "請選擇...", "value": "0" },
        { "txt": "公關單", "value": "1" },
        { "txt": "報廢單", "value": "2" }
    ]
});

var typeStore = Ext.create('Ext.data.Store', {
    fields: ['datatxt', 'datavalue'],
    data: [
        { "datatxt": "系統生成", "datavalue": "1" },
       { "datatxt": "人工keyin", "datavalue": "2" },
    ]
});