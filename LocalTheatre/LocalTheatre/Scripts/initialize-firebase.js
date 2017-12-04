// Initialize Firebase
var config = {
    apiKey: "AIzaSyBOmcSLChHXxyHFnbRp6Vfmbi-N19oTyY8",
    authDomain: "localtheatre-6c6db.firebaseapp.com",
    databaseURL: "https://localtheatre-6c6db.firebaseio.com",
    projectId: "localtheatre-6c6db",
    storageBucket: "localtheatre-6c6db.appspot.com",
    messagingSenderId: "783024149439"
};
firebase.initializeApp(config);


// Get elements
var progressBar = document.getElementById('progressBar');
var fileButton = document.getElementById('fileButton');

if (fileButton) {
    var file;

    // Listen for file selection
    fileButton.addEventListener('change',
        function(e) {

            // Get file
            file = e.target.files[0];

            var reader = new FileReader();

            reader.onload = function(e) {
                $('#preview').attr('src', e.target.result);
            }

            reader.readAsDataURL(file);

        });
};


function sendFile() {

    if (file) {

        var uuid = uuidv4() + '-';

        // Create a storage ref
        var storageRef = firebase.storage().ref('blog/' + uuid + file.name);

        // Update progress bar
        var task = storageRef.put(file);

        // Update progress bar
        task.on('state_changed',
            function progress(snapshot) {
                var percentage = (snapshot.bytesTransferred / snapshot.totalBytes) * 100;
                progressBar.style.width = percentage + '%';
                console.log('Upload is ' + percentage + '% done');
                switch (snapshot.state) {
                case firebase.storage.TaskState.PAUSED: // or 'paused'
                    console.log('Upload is paused');
                    break;
                case firebase.storage.TaskState.RUNNING: // or 'running'
                    console.log('Upload is running');
                    break;
                }
            },
            function error(err) {

                // Handle unsuccessful uploads
                console.log('Error: ' + error.code);
            },
            function complete() {

                // Handle successful uploads on complete
                var downloadUrl = task.snapshot.downloadURL;
                sendInputValue(downloadUrl, uuid + file.name, 1000);
            }
        );

    } else {
        try {
            sendInputValue(currentUrl, currentFile, 500);
        } catch (e) {
            sendInputValue(null, null, 500);
        }
    }
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3).replace(/[018]/g,
        c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}

function sendInputValue(_currentUrl, _currentFile, _time) {
    document.querySelector('#fileurl').value = _currentUrl;
    document.querySelector('#filename').value = _currentFile;
    setTimeout(function() { document.getElementById('btnSubmit').click(); }, _time);
}


function deleteFile(d) {

    if (fileToDelete != null) {

        var storageRef = firebase.storage().ref(fileToDelete.name);

        // Create a reference to the file to delete
        var desertRef = storageRef.child("blog/" + fileToDelete);

        // Delete the file
        desertRef.delete().then(function() {
            console.log('File ' + fileToDelete + ' deleted successfully');

        }).catch(function(error) {
            console.log('Error: ' + error.code);
        });

    }

    if (d) {
        setTimeout(function() { document.getElementById('btnSubmit').click(); }, 1000);
    } else {
        setTimeout(function() { document.getElementById('deleteAndRefresh').click(); }, 1000);
    }
}