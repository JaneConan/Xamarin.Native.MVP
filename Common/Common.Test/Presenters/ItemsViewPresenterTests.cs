﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Entities;
using Common.Models;
using Common.Presenters;
using Moq;
using NUnit.Framework;

namespace Common.Test.Presenters
{
    [TestFixture]
    public class ItemsViewPresenter
    {
        Mock<IDataStore<Student>> mockDataStore;
        private IDataStore<Student> model;
        private ItemsViewPresenter<Student> itemsViewPresenter;

        [SetUp]
        public void BeforeEachTest()
        {
            mockDataStore = new Mock<IDataStore<Student>>();
            model = mockDataStore.Object;

            itemsViewPresenter = new ItemsViewPresenter<Student>(model);
        }

        [Test]
        public void Constructor_NotNullElements_Success()
        {
            // Assert
            Assert.NotNull(itemsViewPresenter.Items);
            Assert.AreEqual(0, itemsViewPresenter.Items.Count);
            Assert.NotNull(itemsViewPresenter.AddItemCommand);
            Assert.NotNull(itemsViewPresenter.LoadItemsCommand);
        }

        [Test]
        public void LoadItemsCommand_ReadTwoElements_Success()
        {
            // Arrange
            var expectedResult = new List<Student>
            {
                new Student() { Name = "name", BornDate = "01-01-1970", Country = "country_test" },
                new Student() { Name = "name_1", BornDate = "02-02-1971", Country = "country_test_1" }
            };

            // mock expected result of GetItemsAsync() method
            mockDataStore.Setup(x => x.GetItemsAsync()).Returns(Task.FromResult(expectedResult));

            // Act
            itemsViewPresenter.LoadItemsCommand.Execute(null);

            // Assert
            Assert.AreEqual(expectedResult, itemsViewPresenter.Items);
        }

        /// <summary>
        /// If LoadItemsCommand don't stop when isBusy is true delete viewModel.Items of two elements
        /// and load list on 1 element.
        /// Finally Assert failed.
        /// </summary>
        [Test]
        public void LoadItemsCommand_IsBusy_NothingToDo()
        {
            // Arrange
            itemsViewPresenter.IsBusy = true;

            var items = new List<Student>
            {
                new Student() { Name = "name", BornDate = "01-01-1970", Country = "country_test" }
            };
            // mock expected result of GetItemsAsync() method
            mockDataStore.Setup(x => x.GetItemsAsync()).Returns(Task.FromResult(items));

            itemsViewPresenter.Items.Add(items.First());
            itemsViewPresenter.Items.Add(items.First());

            // Act
            itemsViewPresenter.LoadItemsCommand.Execute(null);

            // Assert
            Assert.AreEqual(2, itemsViewPresenter.Items.Count);
        }

        [Test]
        public void AddItemAsync_AddItem_Success()
        {
            // Arrange
            var expectedResult = new Student() { Name = "name", BornDate = "01-01-1970", Country = "country_test" };

            // mock AddItemAsync() method
            mockDataStore.Setup(x => x.AddItemAsync(expectedResult));

            // Act
            itemsViewPresenter.AddItemCommand.Execute(expectedResult);

            // Assert
            Assert.True(expectedResult.Equals(itemsViewPresenter.Items.First()));
        }

        /// <summary>
        /// If AddItemCommand don't stop when isBusy is true add new element to viewModel.Items empty list
        /// and finally Assert condition failed.
        /// </summary>
        [Test]
        public void AddItemAsync_IsBusyTrue_NothingToDo()
        {
            // Arrange
            itemsViewPresenter.IsBusy = true;
            var item = new Student() { Name = "name", BornDate = "01-01-1970", Country = "country_test" };

            // mock AddItemAsync() method
            mockDataStore.Setup(x => x.AddItemAsync(item));

            // Act
            itemsViewPresenter.AddItemCommand.Execute(item);

            // Assert
            Assert.AreEqual(0, itemsViewPresenter.Items.Count);
        }

        [Test]
        public void DeleteAllAsync_DeleteAllOnNotEmptyList_Success()
        {
            // Arrange
            var items = new List<Student>
            {
                new Student() { Name = "name", BornDate = "01-01-1970", Country = "country_test" },
                new Student() { Name = "name_1", BornDate = "02-02-1971", Country = "country_test_1" },
                new Student() { Name = "name_2", BornDate = "02-02-1972", Country = "country_test_2" }
            };

            // mock AddItemAsync() method for generic input object of Student class
            mockDataStore.Setup(x => x.AddItemAsync(It.IsAny<Student>()));

            items.ForEach(x => itemsViewPresenter.AddItemCommand.Execute(x));
            Assert.AreEqual(items.Count, itemsViewPresenter.Items.Count);

            // mock DeleteAllAsync() method
            mockDataStore.Setup(x => x.DeleteAllAsync());

            // Act
            itemsViewPresenter.DeleteAllCommand.Execute(null);

            // Assert

            // items list of the view model should be empty
            Assert.AreEqual(0, itemsViewPresenter.Items.Count);
        }

        /// <summary>
        /// If DeleteAllCommand don't stop when isBusy is true delete all items from viewModel.Items list
        /// and finally Assert condition failed.
        /// </summary>
        [Test]
        public void DeleteAllAsync_IsBusy_NothingToDo()
        {
            // Arrange
            itemsViewPresenter.IsBusy = true;

            var items = new List<Student>
            {
                new Student() { Name = "name", BornDate = "01-01-1970", Country = "country_test" },
                new Student() { Name = "name_1", BornDate = "02-02-1971", Country = "country_test_1" },
                new Student() { Name = "name_2", BornDate = "02-02-1972", Country = "country_test_2" }
            };
            items.ForEach(x => itemsViewPresenter.Items.Add(x));

            // mock DeleteAllAsync() method
            mockDataStore.Setup(x => x.DeleteAllAsync());

            // Act
            itemsViewPresenter.DeleteAllCommand.Execute(null);

            // Assert
            Assert.AreEqual(items.Count, itemsViewPresenter.Items.Count);
        }
    }
}
