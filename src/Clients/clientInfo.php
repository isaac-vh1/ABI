<?php
$servername = "192.168.0.254";
$username = "isaacvanhorn";
$password = "bakerr1M";
$database = "ABI";
$conn = mysqli_connect($servername, $username, $password, $database);

if (!$conn) {
    die("Connection failed: " . mysqli_connect_error());
}

    
function execute_query($conn, $query) {
    $result = $conn->query($query);

    if ($result === false) {
        echo "Error: " . $conn->error;
        return;
    }

    if ($result->num_rows > 0) {
        echo "<table border='1'>";
        echo "<tr>";
        while ($field_info = $result->fetch_field()) {
            echo "<th>{$field_info->name}</th>";
        }
        echo "</tr>";

        while ($row = $result->fetch_assoc()) {
            echo "<tr>";
            foreach ($row as $value) {
                echo "<td>$value</td>";
            }
            echo "</tr>";
        }
        echo "</table>";
    } else {
        echo "0 results";
    }

    $result->free();
}

if (isset($_GET['relation'])) {
    $relation = $conn->real_escape_string($_GET['relation']);
    $query = "SELECT * FROM $relation";
    execute_query($conn, $query);
} elseif (isset($_GET['query'])) {
    $query_id = $_GET['query'];
    switch ($query_id) {
        case '1':
            $query = "SELECT * FROM clients";
            break;
        case '2':
            $query = "SELECT COUNT(*) AS NumBooksPublishedBefore1940 FROM Publication WHERE Publication_Date < '1940-01-01'";
            break;
        case '3':
            $query = "SELECT Username FROM User WHERE User_ID IN (SELECT User_ID FROM Purchases GROUP BY User_ID HAVING SUM(Purchase_Quantity) > 5)";
            break;
        case '4':
            $query = "SELECT Book_Title FROM Book WHERE Book_ID IN (SELECT Publication.Book_ID FROM Publication INNER JOIN Purchases ON Publication.Publication_ID = Purchases.Publication_ID INNER JOIN User ON Purchases.User_ID = User.User_ID WHERE User.Username = 'alice')";
            break;
        case '5':
            $query = "SELECT AVG(Price) AS AveragePriceAfter2022 FROM Purchases WHERE Purchase_Date > '2022-12-31'";
            break;
    }
    execute_query($conn, $query);
} elseif ($_SERVER["REQUEST_METHOD"] == "POST") {
    $query = $conn->real_escape_string($_POST['query']);
    execute_query($conn, $query);
}

$conn->close();
?>
