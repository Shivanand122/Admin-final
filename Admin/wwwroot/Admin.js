let pendingChanges = [];

$(document).ready(function () {
    // Function to check the feature flag status
    function checkFeatureFlag() {
        $.ajax({
            url: "/AdminM/IsAdminSideEnabled", // Update the URL to your controller action
            type: "GET",
            success: function (result) {
                if (result) {
                    // Feature flag is enabled, show the "Download PDF" button
                    $('#externalPrintButton').show();

                } else {
                    // Feature flag is disabled, hide the "Download PDF" button
                    $('#externalPrintButton').hide();
                }
            },
            error: function (error) {
                console.error("Error checking feature flag:", error);
            }
        });
    }

    // Check the feature flag status every 5 seconds (adjust as needed)
    setInterval(checkFeatureFlag, 1000);

    // Initial check when the page loads
    checkFeatureFlag();
});





function loadFeatureDetails(featureId) {
    fetch(`/AdminM/FeatureDetails?featureId=${featureId}`)
        .then(response => response.text())
        .then(data => {
            // Display feature details in the preview pane
            document.getElementById('featurePreviewPane').innerHTML = data;
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}


function openFeatureDetailsModal(featureId) {
    fetch(`/AdminM/FeatureDetails?featureId=${featureId}`)
        .then(response => response.text())
        .then(data => {
            $('#featureDetailsModalContent').html(data);
            $('#featureDetailsModal').modal('show');
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
        });
}

// Remove the closeFeatureDetailsPopup function as it is not needed for Bootstrap modal.






function addChange(featureId, action, comment = '') {
   
    const existingChangeIndex = pendingChanges.findIndex(
        (change) => change.featureId === featureId
    );

    // If the change exists and the action is 'Accept' or 'Reject', update the action only
    if (existingChangeIndex !== -1 && (action === 'Accept' || action === 'Reject')) {
        pendingChanges[existingChangeIndex].action = action;
    }


    else {
    
        // Add a new change for any action other than 'Accept' or 'Reject'
        pendingChanges.push({ featureId, action, comment });
    }

    // Update the pending changes preview in the modal without calling another function
    var previewContent = '<h5>Changes Preview:</h5><ul>';
    pendingChanges.forEach((change) => {
        previewContent += `<li>${change.action} for feature ID ${change.featureId} ${change.comment}`;
        if (change.comment) {
            previewContent += ` - Comment: ${change.comment}`;
        }
        previewContent += `</li>`;
        // If there's a comment, include it in the preview for each change
        if (change.comment) {
            previewContent += `<li>Comment: ${change.comment}</li>`;
        }
    });
    previewContent += '</ul>';

    var previewBody = document.getElementById('previewChangesBody');
    if (previewBody) {
        previewBody.innerHTML = previewContent;
    }
}



function loadFeatureDetails(featureId) {
    const previewPane = document.getElementById('featurePreviewPane');
    const commentBoxContainer = document.getElementById('commentBoxContainer');
    var section = document.querySelector('.features-section');
    var section2 = document.querySelector('.bar-section');
    section.style.transform = 'translateX(-10%)';
    section2.style.transform = 'translateX(-10%)';
    // Move the section to the
    // Function to load comment box content
    function loadCommentBox(featureId) {
        fetch(`/AdminM/CommentBox?featureId=${featureId}`)
            .then(response => response.text())
            .then(commentBoxData => {
                commentBoxContainer.innerHTML = commentBoxData;
                commentBoxContainer.style.display = 'block'; // Display the comment box after loading its content

                // Optionally show the comment box if needed based on your UI logic
            })
            .catch(error => {
                console.error('There was a problem loading the comment box:', error);
            });
    }

    // Load feature details
    fetch(`/AdminM/FeatureDetails?featureId=${featureId}`)
        .then(response => response.text())
        .then(featureData => {
            previewPane.innerHTML = featureData;
            previewPane.style.display = 'block';

            // Load the comment box after loading the feature details
            loadCommentBox(featureId);
        })
        .catch(error => {
            console.error('There was a problem loading the feature details:', error);
        });
}


function closeCommentBox() {
    const commentBoxContainer = document.getElementById('commentBoxContainer');
    const featurePreviewPane = document.getElementById('featurePreviewPane');
    var section = document.querySelector('.features-section');
    var section2 = document.querySelector('.bar-section');
    // Hide the comment box container and feature preview pane
    if (commentBoxContainer) {
        commentBoxContainer.style.display = 'none';
    }
    if (featurePreviewPane) {
        featurePreviewPane.style.display = 'none';

        section.style.transform = 'translateX(0%)';
        section2.style.transform = 'translateX(0%)';

    }
}







function saveComment(featureId) {
    var commentTextArea = document.getElementById(`commentTextArea_${featureId}`);
    var comment = commentTextArea.value.trim();
    console.log('Feature ID:', featureId);

    // Show SweetAlert2 confirmation dialog
    Swal.fire({
        title: "Are you sure?",
        text: "You want to add comment?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, add comment!"
    }).then((result) => {
        if (result.isConfirmed) {
            // User clicked "Yes"

            console.log(featureId);
            fetch(`/AdminM/UpdateComment?featureId=${featureId}&comment=${encodeURIComponent(comment)}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    if (response.ok) {
                        // Comment accepted successfully
                        addChange(featureId, 'Comment',comment);



                        //pendingChanges.push({ featureId, comment,'Comment' });

                        $('#commentModal').modal('hide');
                        Swal.fire({
                            title: "Comment Added!",
                            text: "Your comment has been added.",
                            icon: "success",
                            timer: 2000,
                            showConfirmButton: false
                        });
                    } else {
                        console.error('Failed to comment feature');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    });
}

function acceptFeature(featureId) {
  
    var acceptButton = document.querySelector(`button[data-feature-id="${featureId}"][onclick^="acceptFeature"]`);
    var rejectButton = document.querySelector(`button[data-feature-id="${featureId}"][onclick^="rejectFeature"]`);

    Swal.fire({
        title: "Are you sure?",
        text: "You want to accept the feature?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#28a745", // green color for the confirm button
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, accept it!"
    }).then((result) => {
        if (result.isConfirmed) {
            if (acceptButton && acceptButton.innerText !== 'Approved') {
                acceptButton.innerText = 'Approved';
                acceptButton.classList.add('btn-outline-success');
                acceptButton.classList.remove('btn-outline-primary');
            }

            if (rejectButton && rejectButton.innerText !== 'Reject') {
                rejectButton.innerText = 'Reject';
                rejectButton.classList.remove('d-none');
                rejectButton.classList.remove('btn-outline-danger');
                rejectButton.classList.add('btn-outline-primary');
            }

            fetch(`/AdminM/AcceptFeature?featureId=${featureId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    if (response.ok) {
                        console.log('Feature accepted successfully');
                        addChange(featureId, 'Accept');
                    } else {
                        console.error('Failed to accept feature');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    });
}

function rejectFeature(featureId) {
   
    var acceptButton = document.querySelector(`button[data-feature-id="${featureId}"][onclick^="acceptFeature"]`);
    var rejectButton = document.querySelector(`button[data-feature-id="${featureId}"][onclick^="rejectFeature"]`);

    Swal.fire({
        title: "Are you sure?",
        text: "You want to reject the feature?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "Yes, reject it!"
    }).then((result) => {
        if (result.isConfirmed) {
            if (rejectButton && rejectButton.innerText !== 'Rejected') {
                rejectButton.innerText = 'Rejected';
                rejectButton.classList.add('btn-outline-danger');
                rejectButton.classList.remove('btn-outline-primary');
            }

            if (acceptButton && acceptButton.innerText !== 'Accept') {
                acceptButton.innerText = 'Accept';
                acceptButton.classList.remove('btn-outline-success');
                acceptButton.classList.add('btn-outline-primary');
            }

            fetch(`/AdminM/RejectFeature?featureId=${featureId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    if (response.ok) {
                        console.log('Feature rejected successfully');
                        addChange(featureId, 'Reject');
                    } else {
                        console.error('Failed to reject feature');
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    });
}

function updateAllChanges() {
    // Create a preview of changes
    var previewContent = '';
    pendingChanges.forEach(change => {
        previewContent += `<li>${change.action} for feature ID ${change.featureId} ${change.comment}</li>`;
    });
    previewContent += '</ul>';

    // Set the preview content inside the modal body
    var previewBody = document.getElementById('previewChangesBody');
    if (previewBody) {
        previewBody.innerHTML = previewContent;
    }

    // Show the modal
    $('#previewModal').modal('show');

    // Handle the OK button click to proceed with updates
    $('#confirmUpdateButton').on('click', function () {
        // Assuming you want to send changes to the server using fetch
        fetch('/AdminM/UpdateAllChanges', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pendingChanges)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                // Handle success, e.g., clear changes or update UI to reflect changes
                pendingChanges = []; // Clear changes after successful update
                $('#previewModal').modal('hide'); // Close the modal
                Swal.fire({
                    title: 'Success!',
                    text: 'All changes have been successfully updated.',
                    icon: 'success',
                    timer: 1300, // Show for 2 seconds
                    showConfirmButton: false

                }).then(() => {
                    location.reload()
                    // After the timer finishes, reload the page
                    // location.reload();
                });




            })
            .catch(error => {
                // Handle errors
                console.error('There was a problem with the fetch operation:', error);
            });
    });

    // Handle the Cancel button click to close the modal
    $('#cancelUpdateButton').on('click', function () {
        $('#previewModal').modal('hide'); // Close the modal
    });

    // Handle the "Clear All" button click to clear all changes and the preview content
    $('#clearAllChangesButton').on('click', function () {
        fetch('/AdminM/ClearPendingChanges', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                // Optional: Handle UI update or other tasks after clearing
                console.log('Pending changes cleared successfully');

            })
            .catch(error => {
                // Handle errors
                console.error('There was a problem with the fetch operation:', error);
            });

        pendingChanges = []; // Clear pending changes
        $('#previewChangesBody').empty(); // Clear the preview content
        $('#previewModal').modal('hide'); // Close the modal
        location.reload(); // Refresh the page

    });
}




function initializeChart(data) {
    console.log('Initializing chart with data:', data);
    var ctx = document.getElementById('barChart').getContext('2d');
    try {
        return new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Approved', 'Rejected', 'Pending'],
                datasets: [{
                    label: 'Count',
                    data: [data.approved.length, data.rejected.length, data.pending.length],
                    backgroundColor: [
                        'rgba(0, 128, 0, 1)',    // Green with full opacity
                        'rgba(255, 0, 0, 0.5)',  // Red with 50% opacity
                        'rgba(0, 64, 128, 0.6)', // Blue with 60% opacity
                    ],
                    borderColor: 'rgba(0, 0, 255, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: {
                        beginAtZero: true,
                        max: Math.max(data.approved.length, data.rejected.length, data.pending.length) + 1
                    }
                },
                legend: {
                    display: false,
                }
            }
        });
    } catch (error) {
        console.error('Error initializing chart:', error);
    }
}

// Function to update the total counts
function updateTotalCounts(data) {
    var totalCountsElement = document.getElementById('totalCounts');
    if (totalCountsElement) {
        var totalCount = data.approved.length + data.rejected.length + data.pending.length;
        totalCountsElement.innerText = `Total Counts: ${totalCount}`;
    }
}

// Ajax function
function fetchDataAndInitializeChart() {
    $.ajax({
        url: 'https://oninechartapi.azurewebsites.net/api/Common/GetApprovalStatuses',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var myChart = initializeChart(data);

            // Fetch new data every 5 seconds and update the chart
            setInterval(function () {
                $.ajax({
                    url: 'https://oninechartapi.azurewebsites.net/api/Common/GetApprovalStatuses',
                    type: 'GET',
                    dataType: 'json',
                    success: function (newData) {
                        if (myChart) {
                            myChart.data.datasets[0].data = [
                                newData.approved.length,
                                newData.rejected.length,
                                newData.pending.length
                            ];
                            myChart.options.scales.y.max = Math.max(...myChart.data.datasets[0].data) + 1;
                            myChart.update();

                            // Update total counts
                            updateTotalCounts(newData);
                        } else {
                            console.error('Chart is undefined.');
                        }
                    },
                    error: function (error) {
                        console.error('Error fetching new data:', error);
                    }
                });
            }, 1000);

            // Update total counts initially
            updateTotalCounts(data);
        },
        error: function (error) {
            console.error('Error fetching initial data:', error);
        }
    });
}

// Call the function to fetch and initialize the chart
fetchDataAndInitializeChart();





// Initialize DataTables
var table;

jQuery(document).ready(function ($) {
    table = $('#featuresTable').DataTable({
        "pageLength": 100,
        "ordering": true,
        "responsive": true,
        "dom": '<"top">rt<"bottom"ip>',
        "buttons": [
            {
                extend: 'print',
                text: '<button class="btn btn-primary">Print</button>',
                customize: function (win) {
                    // Exclude columns from print view
                    $(win.document.body).find('table thead tr th:nth-child(6), table thead tr th:nth-child(7), table tbody tr td:nth-child(6), table tbody tr td:nth-child(7)').remove();
                }
            }
        ],
    });

    $('#externalSearchBox').on('input', function () {
        table.search(this.value).draw();
    });

    // Trigger DataTable print when external button is clicked
    $('#externalPrintButton').on('click', function () {
        table.button('.buttons-print').trigger();
    });
});