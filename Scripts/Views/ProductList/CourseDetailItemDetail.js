var courseDetailItemStore;
var courseDetailItemGrid;
function initCoursePanel(product_id) {
    courseDetailItemStore = Ext.create('Ext.data.Store', {
        fields: [
             { name: 'Course_Detail_Name', type: 'string' },
             { name: 'Spec_Name1', type: 'string' },
             { name: 'Spec_Name2', type: 'string' },
              {
                  name: 'Spec_Name', type: 'string', convert: function (v, record) {
                      if (!record.data.Spec_Name1 && !record.data.Spec_Name2) {
                          return PRODUCT_NON_SPEC;
                      }
                      return record.data.Spec_Name1 + ' ' + record.data.Spec_Name2;
                  }
              }
             //{ name: 'Ticket_Count', type: 'int' }
        ],
        proxy: {
            type: 'ajax',
            url: '/Product/CourseDetailItemQuery?detail=true',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    courseDetailItemStore.load({ params: { ProductId: product_id } });

    courseDetailItemGrid = Ext.create('Ext.grid.Panel', {
        plugins: [{ ptype: 'cellediting' }],
        title: CURRICULUM_SETTING,
        store: courseDetailItemStore,
        margin: '0 30 0 0',
        x: 20,
        y:20,
        width: 380,
        height: 300,
        columns: [
            { xtype: 'rownumberer', width: 30 },
            { text: PRODUCT_DETAIL, dataIndex: 'Spec_Name', flex: 1, menuDisabled: true },
            { text: CURRICULUM_DETAIL, dataIndex: 'Course_Detail_Name', flex: 1, menuDisabled: true }
        ]
    });
}