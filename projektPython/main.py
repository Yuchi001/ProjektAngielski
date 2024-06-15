import pandas as pd
import matplotlib.pyplot as plt
import seaborn as sb
from pandas import DataFrame


# Zaimportować dane z pliku data.csv do obiektu DataFrame.
# Wyświetlić zaimportowane dane.
# Wyświetlić liczbę kolumn i liczbę wierszy obiektu DataFrame.
# Wyświetlić listę nazw kolumn obiektu DataFrame.
def print_general_info(data: DataFrame):
    # Wyświetlenie danych z pliku data.csv oraz wyswietlenie ilości kolumn i wierszy
    print("Data:\n", data, end="\n\n")
    print("Column names:", data.columns.values, end="\n")


# Dla każdej kolumny proszę wyznaczyć i wyświetlić:
# wartość średnią, medianę, minimum, maksimum.
# Dla każdej z kolumn proszę wyświetlić wykres pudełkowy.
def print_columns_statistics(data: DataFrame):
    # Wyświetlanie statystyk dla każdej kolumny
    for col in data.columns:
        print(f"\nStats for column {col}:")
        print("Average: ", data[col].mean())
        print("Median: ", data[col].median())
        print("Minimum: ", data[col].min())
        print("Maximum: ", data[col].max())

    # Tworzenie i wyświetlanie wykresów pudełkowych dla każdej kolumny
    for col in data.columns:
        plt.figure(figsize=(8, 6))
        sb.boxplot(y=data[col])
        plt.title(f'Boxplot for column {col}')
        plt.show()


# Wyznaczyć i wyświetlić macierz korelacji,
# wyświetlić wykres „mapy ciepła” dla wyznaczonej macierzy korelacji.
def print_detailed_info(data: DataFrame):
    correlation_matrix = data.corr()

    # Wyświetlanie macierzy korelacji
    print("\nCorrelation matrix:\n", correlation_matrix)

    # Tworzenie i wyświetlanie mapy ciepła
    plt.figure(figsize=(10, 8))
    sb.heatmap(correlation_matrix, annot=True, cmap='coolwarm')
    plt.title('Heatmap for correlation matrix')
    plt.show()


# Dodatkowe operacje w tym:
# Wykres skrzypcowy: https://seaborn.pydata.org/generated/seaborn.violinplot.html
# Wykres gęstości KDE: https://seaborn.pydata.org/generated/seaborn.kdeplot.html
def additional_operations(data: DataFrame):
    # Tworzenie i wyświetlanie wykresów gęstości (KDE)
    for col in data.columns:
        plt.figure(figsize=(8, 6))
        sb.kdeplot(data[col], shade=True)
        plt.title(f'KDE plot for column {col}')
        plt.show()

    # Tworzenie i wyświetlanie wykresów skrzypcowych
    for col in data.columns:
        plt.figure(figsize=(8, 6))
        sb.violinplot(y=data[col])
        plt.title(f'Violin plot for column {col}')
        plt.show()


def main():
    data = pd.read_csv('data.csv')
    print_general_info(data)
    print_columns_statistics(data)
    print_detailed_info(data)


if __name__ == "__main__":
    main()
