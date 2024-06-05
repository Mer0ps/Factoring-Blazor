// Dans un fichier JavaScript (par exemple, pdfLoader.js)
window.loadPdf = function (pdfUrl) {
    const loadingTask = pdfjsLib.getDocument(pdfUrl);
    loadingTask.promise.then(function (pdf) {
        // Chargement réussi
        pdf.getPage(1).then(function (page) {
            var scale = 1.5;
            var viewport = page.getViewport({ scale: scale, });
            var canvas = document.createElement('canvas');
            var context = canvas.getContext('2d');
            canvas.height = viewport.height;
            canvas.width = viewport.width;
            var renderContext = {
                canvasContext: context,
                viewport: viewport,
            };
            page.render(renderContext);
            document.getElementById('pdfViewer').appendChild(canvas);
        });
    }, function (reason) {
        // Erreur lors du chargement du PDF
        console.error('Unable to load PDF', reason);
    });
};