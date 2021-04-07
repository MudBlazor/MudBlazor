class MudEnhancedChartHelper {

    constructor() {
    }

    triggerAnimation(chartid) {
        var elements = document.getElementsByTagName('polygon');

        for (var i = 0; i < elements.length; i++) {
            if (elements[i].getAttribute('data-chartid') == chartid) {
                elements[i].firstChild.beginElement();
            }
        }
    }
   
};

window.mudEnhancedChartHelper = new MudEnhancedChartHelper();